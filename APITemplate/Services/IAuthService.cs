using APITemplate.Constants;
using APITemplate.DBModels;
using APITemplate.Models;

namespace APITemplate.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> LoginAsync(LoginModel model);
        Task<ApplicationUser> AssginRoleAsync(UserRoles role, string userID);
        Task<ApplicationUser> RevokeRoleAsync(List<UserRoles> roles, string userID);
        Task<List<string>> GetUserRolesAsync(string userID);
    }
}
