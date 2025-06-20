using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using newportal.DataAccess.Data;
using newportal.DataAccess.Repository.IRepository;
using System.Security.Claims;

namespace newportal.DataAccess.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbcontext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly EmailSender _emailSender;
        private readonly PhoneSender _phoneSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthRepository(ApplicationDbcontext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, EmailSender emailSender, PhoneSender phoneSender, IHttpContextAccessor httpContextAccessor)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _phoneSender = phoneSender;
            _httpContextAccessor = httpContextAccessor;
        }





       

        public async Task<bool> SendPhoneOtpAsync(string phone)
        {
            var formattedPhone = "+91" + phone;
            return await _phoneSender.SendOtpAsync(formattedPhone);
        }

        public bool VerifyPhoneOtp(string phone, string otp)
        {
            return _phoneSender.VerifyOtp(phone, otp);
        }

        public async Task<bool> SendEmailOtpAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            await _emailSender.SendOtpAsync(email);
            return true;
        }

        public bool VerifyEmailOtp(string otp)
        {
            if (string.IsNullOrEmpty(otp)) return false;

            bool isVerified = _emailSender.VerifyOtp(otp);

            if (isVerified)
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Additional logic like marking email as verified can go here
            }

            return isVerified;
        }


        public Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            return _emailSender.SendEmailAsync(toEmail, subject, htmlMessage);
        }


        public async Task<IdentityUser?> FindByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<bool> CheckPasswordAsync(IdentityUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> IsLockedOutAsync(IdentityUser user)
        {
            return await _userManager.IsLockedOutAsync(user);
        }

        public async Task<bool> GetTwoFactorEnabledAsync(IdentityUser user)
        {
            return await _userManager.GetTwoFactorEnabledAsync(user);
        }

        public async Task<SignInResult> PasswordSignInAsync(string username, string password, bool rememberMe, bool lockoutOnFailure)
        {
            return await _signInManager.PasswordSignInAsync(username, password, rememberMe, lockoutOnFailure);
        }

        public async Task SignInAsync(IdentityUser user, bool rememberMe)
        {
            await _signInManager.SignInAsync(user, rememberMe);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IEnumerable<string>> GetRolesAsync(IdentityUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task AccessFailedAsync(IdentityUser user)
        {
            await _userManager.AccessFailedAsync(user);
        }
    }
}
