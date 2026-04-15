using Loan_Processing_Inzamam.Models;
using Microsoft.AspNetCore.Identity;

namespace Loan_Processing_Inzamam.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Seed Roles
            string[] roleNames = { "Admin", "Employee", "Client" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Seed Default Admin User
            string adminEmail = "inzamamkhan71@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    IsProfileComplete = true // Bypasses the initial setup screen
                };

                // Create the user with a strong default password
                var createPowerUser = await userManager.CreateAsync(newAdmin, "Inzamamkhan71@");
                if (createPowerUser.Succeeded)
                {
                    // Assign the "Admin" role to this user
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}