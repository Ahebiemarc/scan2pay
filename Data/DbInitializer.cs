using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using scan2pay.Models;

namespace scan2pay.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Créer les rôles s'ils n'existent pas
            string[] roleNames = { "Admin", "Client", "Marchand" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Créer un utilisateur admin par défaut (optionnel)
            var adminUser = await userManager.FindByEmailAsync("admin@scan2pay.com");
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@scan2pay.com",
                    Email = "admin@scan2pay.com",
                    FirstName = "Admin",
                    LastName = "Scan2Pay",
                    UserType = UserType.Client, // Ou un type spécifique Admin si vous en créez un
                    EmailConfirmed = true // Confirmer l'email pour éviter les étapes de confirmation en dev
                };
                var result = await userManager.CreateAsync(user, "AdminPassword123!"); // Mettez un mot de passe fort
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
             }
        }
        
    }
}