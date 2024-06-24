using APITemplate.Data;
using APITemplate.DBModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Web;

namespace APITemplate.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationUser? _user;

        public UserService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _user = _userManager.Users.SingleOrDefault(u => u.Id == CurrentUserID);
        }

        public string CurrentUserID
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            }
        }

        public ApplicationUser? CurrentUser
        {
            get
            {
                return _user;
            }
        }

        public ApplicationUser? GetUserByID(string userID)
        {
            return _userManager.Users.SingleOrDefault(u => u.Id == userID);
        }

        public bool CheckIfModelBelongToCurrentUser(string modelUserID)
        {
            var userID = CurrentUserID;
            if (string.IsNullOrEmpty(userID))
            {
                return false;
            }

            var user = _userManager.Users.SingleOrDefault(u => u.Id == userID);

            if (user == null)
            {
                return false;
            }

            return modelUserID == userID;
        }

        public async Task<bool> IsAdmin()
        {
            var user = CurrentUser;
            if (user == null)
            {
                return false;
            }

            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<bool> IsUser()
        {
            var user = CurrentUser;
            if (user == null)
            {
                return false;
            }

            return await _userManager.IsInRoleAsync(user, "User");
        }
    }
}
