using Microsoft.AspNetCore.Identity;


namespace CampusTouch.Infrastructure.Persistance.Seed
{
    public  class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Student", "Staff" ,"Applicant"};

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
