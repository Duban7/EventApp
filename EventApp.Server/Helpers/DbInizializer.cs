using Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EventApp.Helpers
{
    public static class DbInitializer
    {
        public static async Task SeedUsersAndRolesAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminEmail = "admin@mail.com";
            string adminPassword = "admin1337";

            string[] roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Admin",
                    Surname = "Admin",
                    BirthDate = new DateTime(2003, 9, 19),
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    var foundAdmin = await userManager.FindByEmailAsync(adminEmail) ?? throw new Exception("User not found");

                    Claim[] userClaims = [
                        new Claim(ClaimTypes.NameIdentifier, foundAdmin.Id!),
                        new Claim(ClaimTypes.Email, foundAdmin.Email!),
                        new Claim(ClaimTypes.Name, foundAdmin.Name!),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim(ClaimTypes.Role, "User")
                    ];

                    var res = await userManager.AddClaimsAsync(foundAdmin, userClaims);
                    if (!res.Succeeded) throw new Exception("Couldn't add Claims");
                    await userManager.AddToRoleAsync(admin, "Admin");
                    await userManager.AddToRoleAsync(admin, "User");

                    return;
                }
                else
                {
                    throw new Exception("Не удалось создать администратора: " +
                        string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

}
