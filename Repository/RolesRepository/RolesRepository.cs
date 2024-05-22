using E_CommerceApi.Models.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace E_CommerceApi.Repository.RolesRepository.RolesRepository
{
    public class RolesRepository : IRolesRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // Get All Roles
        public async Task<List<string>> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            List<string> roleNames = new List<string>();
            foreach (var role in roles)
            {
                roleNames.Add(role.Name);
            }
            return roleNames;
        }
        // Add new Role
        public async Task<bool> AddRoleAsync(string Name)
        {
            Name = char.ToUpper(Name[0]) + Name.Substring(1);

            if (await _roleManager.RoleExistsAsync(Name))
                return false;

            await _roleManager.CreateAsync(new IdentityRole(Name));
            return true;
        }
        //Delete Role From Roles List
        public async Task<bool> DeleteRole(string Name)
        {
            var role = await _roleManager.FindByNameAsync(Name);
            if (role is not null)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                if (!usersInRole.Any())
                {
                    await _roleManager.DeleteAsync(role);
                    return true;
                }
            }
            return false;
        }
    }
}
