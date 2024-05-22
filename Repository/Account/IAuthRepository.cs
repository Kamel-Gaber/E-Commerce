using E_CommerceApi.Models;
using E_CommerceApi.Models.Account;
using E_CommerceApi.Models.DbContext;

namespace E_CommerceApi.Repository.Account
{
    public interface IAuthRepository
    {
        Task<AuthModel> RegisterAsync(SignUpModel model);
        Task<AuthModel> GetTokenAsync(TokenRequistModel model);
        Task<string> AddRoleAsync(AddRoleToUserModel model);
        Task<string> GetNameOfUser(string Id);
        Task<ApplicationUser> GetUserById(string Id);
    }
}
