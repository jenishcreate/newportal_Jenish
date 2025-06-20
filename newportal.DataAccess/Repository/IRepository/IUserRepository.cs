using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using newportal.Models;

namespace newportal.DataAccess.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<string> CreateUserByAdminAsync(ApplicationUser userVM);
        Task<UserNameAndCompanyVM> masterdistributorchind(string userId, string rolename);
        Task<IdentityUser> CurrentUser(string currentuserId);
        Task<ApplicationUser> CurrentUserData(string currentuserId);
        Task<UserNameAndCompanyVM> parentUserCombination(string userId, string parentId, string rolename);
        Task<string> UpdateProfilePictureAsync(string userId, IFormFile profileImage);
        Task<bool> DeleteProfileImageAsync(string fullPathToFile);
        Task<UserData> GetKycDetailsByUserId(string userId);
        string? GetProfilePictureUrlByUserId(string userId);
        string getParentId(string userId);
        string getL2ParentId(string userId);
        Task<Task> Logout();
        Task<IdentityUser> FindByEmailAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string code, string newPassword);
        Task<bool> IsTwoFactorEnabledAsync(string userId);
        Task<bool> SetTwoFactorEnabledAsync(string userId, bool isEnabled);
        Task<IdentityUser> CurrentUserByUsername(string currentuserId);
        Task<bool> IsEmailConfirmedAsync(IdentityUser user);
        Task<string> GeneratePasswordResetTokenAsync(IdentityUser user);
        Task<bool> ConfirmEmailManuallyAsync(string username);

        Task<bool> HasPasswordAsync(IdentityUser user);

        Task<IdentityResult> ChangePasswordAsync(IdentityUser user, string currentPassword, string newPassword);
        Task RefreshSignInAsync(IdentityUser user);
        Task<bool> ChangeTPIN(string userId, string newTpin);




    }
}
