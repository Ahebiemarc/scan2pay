// File: Services/AuthService.cs
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using scan2pay.DTOs;
using scan2pay.Interfaces;
using scan2pay.Models;

namespace scan2pay.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager; // Ajouté
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration; // Ajouté pour la config
    private readonly IMapper _mapper;
    private readonly IWalletService _walletService;
    private readonly IQrCodeService _qrCodeService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, // Ajouté
        ITokenService tokenService,
        IConfiguration configuration, // Ajouté
        IMapper mapper,
        IWalletService walletService,
        IQrCodeService qrCodeService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager; // Ajouté
        _tokenService = tokenService;
        _configuration = configuration; // Ajouté
        _mapper = mapper;
        _walletService = walletService;
        _qrCodeService = qrCodeService;
        _logger = logger;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterUserDto registerDto)
    {
        var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
        if (userExists != null) return null;

        var user = _mapper.Map<ApplicationUser>(registerDto);
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) return null;

        await _userManager.AddToRoleAsync(user, registerDto.UserType);
        await _walletService.CreateWalletForUserAsync(user);

        if (user.UserType == UserType.Marchand)
        {
            var qrCode = await _qrCodeService.GenerateUniqueQrCodeForMarchandAsync(user);
            if (qrCode == null)
            {
                _logger.LogError($"CRITICAL: Failed to create QR Code for new marchand {user.Email}.");
                // Gérer l'échec
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateJwtToken(user, roles);

        var tokenDuration = double.Parse(_configuration["JwtSettings:DurationInMinutes"] ?? "60");

        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserType = user.UserType.ToString(),
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(tokenDuration) // Correction ici
        };
    }
    
    public async Task<AuthResponseDto?> LoginAsync(LoginUserDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateJwtToken(user, roles);

        var tokenDuration = double.Parse(_configuration["JwtSettings:DurationInMinutes"] ?? "60");

        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserType = user.UserType.ToString(),
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(tokenDuration) // Correction ici
        };
    }


    
     public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;
        return _mapper.Map<UserProfileDto>(user);
    }

    public async Task<IdentityResult> UpdateProfileAsync(string userId, UpdateUserProfileDto profileDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        _mapper.Map(profileDto, user); // AutoMapper met à jour les propriétés de 'user'
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
    }
    public async Task<IdentityResult> RequestPasswordResetAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Ne pas révéler si l'utilisateur existe ou non pour des raisons de sécurité
            return IdentityResult.Success; // Ou un message générique
        }
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        // TODO: Envoyer le token par email à l'utilisateur
        // Exemple: await _emailSender.SendPasswordResetEmailAsync(email, token);
        _logger.LogInformation($"Password reset token for {email}: {token}"); // Pour le dev
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            // Ne pas révéler si l'utilisateur existe ou non
            return IdentityResult.Failed(new IdentityError { Description = "Invalid request." });
        }
        return await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
    }
}