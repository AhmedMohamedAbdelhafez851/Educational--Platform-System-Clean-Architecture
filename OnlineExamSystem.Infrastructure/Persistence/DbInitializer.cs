using Microsoft.AspNetCore.Identity;
using OnlineExamSystem.Domains.Entities;
namespace OnlineExamSystem.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var email = "admin@site.com";
            var password = "Admin@123";

            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, password);
            }
        }
    }
}