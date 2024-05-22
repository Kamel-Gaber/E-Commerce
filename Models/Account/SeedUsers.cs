/*using E_CommerceApi.Models.DbContext;
using E_CommerceApi.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace E_CommerceApi.Models.Account
{
    public class SeedUsers
    {
        private static string Password { get; set; } = "123456";
        public static async Task SeedingUsers(UserManager<ApplicationUser> userManager)
        {
            var User1 = new ApplicationUser
            {
                UserName = "amr3ita",
                FirstName = "Amr",
                LastName = "Walied",
                Email = "amrwalied90@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            if (userManager.Users.All(u => u.Id != User1.Id))
            {
                var user = await userManager.FindByEmailAsync(User1.Email);
                if (user is null)
                {
                    await userManager.CreateAsync(User1);
                    await userManager.AddPasswordAsync(User1, Password);
                    await userManager.AddToRoleAsync(User1, Roles.SuperAdmin.ToString());
                }
            }
        }
    }
}
*/