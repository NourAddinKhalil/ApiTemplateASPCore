using APITemplate.DBModels;

namespace APITemplate.Services
{
    public interface IUserService
    {
        string CurrentUserID { get; }
        ApplicationUser? CurrentUser { get; }
        ApplicationUser? GetUserByID(string userID);
        bool CheckIfModelBelongToCurrentUser(string modelUserID);
        Task<bool> IsAdmin();
        Task<bool> IsUser();
    }
}