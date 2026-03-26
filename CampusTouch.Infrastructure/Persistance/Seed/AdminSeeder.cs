using CampusTouch.Infrastructure.Persistance.Identity;
using Microsoft.AspNetCore.Identity;


namespace CampusTouch.Infrastructure.Persistance.Seed
{
    
        public static class AdminSeeder
        {
            public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
            {
                // 🔥 Default Admin Credentials
                var adminEmail = "admin@gmail.com";
                var adminPassword = "Admin@123";

                // ✅ Check if admin already exists
                var existingUser = await userManager.FindByEmailAsync(adminEmail);

                if (existingUser != null)
                    return; // Admin already exists

                // ✅ Create new Admin user
                var admin = new ApplicationUser
                {
                    FullName = "System Admin",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Admin creation failed: {errors}");
                }

                // ✅ Assign Admin role
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }


