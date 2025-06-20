using Microsoft.AspNetCore.Identity;

namespace newportal.DataAccess.Repository.IRepository
{
    public interface IAuthRepository
    {
        
        Task<bool> SendPhoneOtpAsync(string phone);
        bool VerifyPhoneOtp(string phone, string otp);
        Task<bool> SendEmailOtpAsync(string email);
        bool VerifyEmailOtp(string otp);
        Task<IdentityUser?> FindByUsernameAsync(string username);
        Task<bool> CheckPasswordAsync(IdentityUser user, string password);
        Task<bool> IsLockedOutAsync(IdentityUser user);
        Task<bool> GetTwoFactorEnabledAsync(IdentityUser user);
        Task<SignInResult> PasswordSignInAsync(string username, string password, bool rememberMe, bool lockoutOnFailure);
        Task SignInAsync(IdentityUser user, bool rememberMe);
        Task SignOutAsync();
        Task<IEnumerable<string>> GetRolesAsync(IdentityUser user);
        Task AccessFailedAsync(IdentityUser user);
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }
}
