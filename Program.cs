using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using scan2pay.Data;
using scan2pay.Models;
using scan2pay.Services;
using scan2pay.Interfaces;
using System.Text;
using FluentValidation.AspNetCore;
using FluentValidation; // Ajout pour AddValidatorsFromAssemblyContaining
using scan2pay.Repositories;
using scan2pay.Validation; // Assurez-vous que ce namespace existe pour vos validateurs
using scan2pay.Mappings; // Assurez-vous que ce namespace existe pour AutoMapperProfile

var builder = WebApplication.CreateBuilder(args);

// --- Configuration des services ---

// 1. Entity Framework Core & PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Mettre à true pour la production avec confirmation email
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    // options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Configuration du verrouillage
    // options.Lockout.MaxFailedAccessAttempts = 5;
    // options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders(); // Pour la génération de tokens (reset password, 2FA)

// 3. Authentification JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKeyString = jwtSettings["Key"];
if (string.IsNullOrEmpty(secretKeyString))
{
    throw new InvalidOperationException("La clé JWT n'est pas configurée dans JwtSettings:Key.");
}
var secretKey = Encoding.ASCII.GetBytes(secretKeyString);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = builder.Environment.IsProduction(); // false en dev, true en prod
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero // Pas de marge de temps pour l'expiration du token
    };
});

// 4. AutoMapper pour les DTOs
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly); // Scanne l'assembly où se trouve AutoMapperProfile

// 5. Injection de Dépendances (Repositories & Services)
// Repositories
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IQrCodeRepository, QrCodeRepository>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
// Ajoutez d'autres repositories ici si nécessaire

// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IQrCodeService, QrCodeService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();


// 6. Controllers & Validation FluentValidation
builder.Services.AddControllers(); // Enregistre les services de contrôleurs

// Configure FluentValidation pour la validation automatique
builder.Services.AddFluentValidationAutoValidation();
// Enregistre tous les validateurs de l'assembly contenant RegisterUserDtoValidator
// (ou une autre de vos classes de validation)
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();
// La validation implicite des propriétés enfants (ImplicitlyValidateChildProperties) est obsolète.
// La validation des propriétés enfants doit maintenant être configurée explicitement dans chaque validateur parent
// en utilisant .SetValidator(new ChildValidator()) pour la propriété enfant concernée.


// 7. Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Scan2Pay API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Veuillez entrer 'Bearer [token]'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }});
});

// 8. CORS (Cross-Origin Resource Sharing) - Configurez selon vos besoins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", // Nommez votre politique
        policy =>
        {
            // Mettez à jour avec les URL réelles de votre application Expo en dev et prod
            policy.WithOrigins("http://localhost:8081", "exp://192.168.X.X:8081", "[https://votre-app-scan2pay.com](https://votre-app-scan2pay.com)") 
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


var app = builder.Build();

// --- Pipeline de requêtes HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Scan2Pay API v1"));
    app.UseDeveloperExceptionPage(); // Plus de détails sur les erreurs en dev
}
else
{
    app.UseExceptionHandler("/Error"); // Page d'erreur générique en prod
    // app.UseHsts(); // HTTP Strict Transport Security - Décommentez si HTTPS est configuré partout
}

// app.UseHttpsRedirection(); // Décommentez si vous forcez HTTPS et avez un certificat valide en dev/prod

app.UseRouting(); // Important: avant UseAuthentication et UseAuthorization

app.UseCors("AllowSpecificOrigin"); // Appliquez votre politique CORS

app.UseAuthentication(); // Authentification d'abord
app.UseAuthorization();  // Puis autorisation

app.MapControllers(); // Mappe les routes des controllers

// Seed de la base de données (optionnel, pour rôles initiaux ou utilisateurs admin)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var dbContext = services.GetRequiredService<ApplicationDbContext>(); // Pour appliquer les migrations

        // Appliquer les migrations au démarrage (utile en dev, attention en prod)
        // await dbContext.Database.MigrateAsync(); // Décommentez si vous voulez appliquer les migrations au démarrage

        await DbInitializer.InitializeAsync(userManager, roleManager); // Créez cette classe pour le seeding
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database or applying migrations.");
    }
}

app.Run();
