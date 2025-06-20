using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using newportal.DataAccess.Repository;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Models.ViewModel.Auth;
using newportal.Utility;
using Newtonsoft.Json;
using System.Text;
using System.Text.Encodings.Web;

namespace newportal.Controllers
{
    public class AuthController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;    

        public Auth_VM? credential { get; set; } = new Auth_VM();
        private readonly IUnitOfWork _unitOfWork;
        public AuthController(IUnitOfWork unitOfWork, SignInManager<IdentityUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
        }



        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            await _unitOfWork.Auth.SignOutAsync(); 
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme); // Clear external cookies
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");


            return View(credential);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Auth_VM model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");



            if (!ModelState.IsValid)
                return View(model);


            var user = await _unitOfWork.Auth.FindByUsernameAsync(model.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Step 2: Check for lockout
            if (await _unitOfWork.Auth.IsLockedOutAsync(user))
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToAction("Lockout", "Account");
            }

            // Step 3: Check password
            if (await _unitOfWork.Auth.CheckPasswordAsync(user, model.Password))
            {

                // Step 4: Two-Factor required?
                if (await _unitOfWork.Auth.GetTwoFactorEnabledAsync(user))
                {


                    Otp_VM Otptrans = new Otp_VM
                    {
                        InputOtp = string.Empty,
                        auth = model,
                        RememberMachine = model.RememberMe,


                    };
                    TempData["Otptrans"] = JsonConvert.SerializeObject(Otptrans);

                    if (!await _unitOfWork.Auth.SendEmailOtpAsync(user.Email))
                    {
                        ModelState.AddModelError(string.Empty, "Failed to send OTP. Please try again.");
                        return View(model);
                    }

                    return RedirectToAction("LoginWith2fa", "Auth");
                }

                // Step 5: Normal login

                var result = await _unitOfWork.Auth.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var roles = await _unitOfWork.Auth.GetRolesAsync(user);


                    if (roles.Contains(Role.Admin))
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                    if (roles.Contains(Role.MasterDistributor))
                        return RedirectToAction("Index", "Dashboard", new { area = "MasterDistributor" });

                    if (roles.Contains(Role.Distributor))
                        return RedirectToAction("Index", "Dashboard", new { area = "Distributor" });

                    if (roles.Contains(Role.Retailer))
                        return RedirectToAction("Index", "Dashboard", new { area = "Retailer" });

                    if (roles.Contains(Role.NoKyc))
                        return RedirectToAction("Index", "Kyc", new { area = "NoKyc" });

                    return LocalRedirect(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction("Lockout", "Account");
                }
            }

            // Step 6: If password check failed, increase lockout count
            await _unitOfWork.Auth.AccessFailedAsync(user);

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }



        public async Task<IActionResult> LoginWith2fa()
        {
            if (TempData["Otptrans"] == null)
            {
                return RedirectToAction("Login"); // Or handle error
            }

            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(Otp_VM Otptrans)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                TempData["Otptrans"] = JsonConvert.SerializeObject(Otptrans);
                return View();
            }

            if (Otptrans?.auth == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                TempData["Otptrans"] = JsonConvert.SerializeObject(Otptrans);
                return View();
            }
            if (string.IsNullOrEmpty(Otptrans.InputOtp) || Otptrans.InputOtp.Length != 6)
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP format. Please enter a 6-digit OTP.");
                TempData["Otptrans"] = JsonConvert.SerializeObject(Otptrans);
                return View();
            }
            if (!_unitOfWork.Auth.VerifyEmailOtp(Otptrans.InputOtp))
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired OTP. Please try again.");
                TempData["Otptrans"] = JsonConvert.SerializeObject(Otptrans);
                return View();
            }

            var user = await _unitOfWork.Auth.FindByUsernameAsync(Otptrans.auth.Username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                TempData["Otptrans"] = JsonConvert.SerializeObject(Otptrans);
                return View();
            }
            await _unitOfWork.User.SetTwoFactorEnabledAsync(user.Id, false);

            var result = await _unitOfWork.Auth.PasswordSignInAsync(
                Otptrans.auth.Username,
                Otptrans.auth.Password,
                Otptrans.auth.RememberMe,
                lockoutOnFailure: true);

            await _unitOfWork.User.SetTwoFactorEnabledAsync(user.Id, true);



            if (result.Succeeded)
            {
                var roles = await _unitOfWork.Auth.GetRolesAsync(user);

                if (roles.Contains(Role.Admin))
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                if (roles.Contains(Role.MasterDistributor))
                    return RedirectToAction("Index", "Dashboard", new { area = "Master_Distributor" });

                if (roles.Contains(Role.Distributor))
                    return RedirectToAction("Index", "Dashboard", new { area = "Distributor" });

                if (roles.Contains(Role.Retailer))
                    return RedirectToAction("Index", "Dashboard", new { area = "Retailer" });

                if (roles.Contains(Role.NoKyc))
                    return RedirectToAction("Index", "Kyc", new { area = "NoKyc" });

                // Default fallback if no role matched
                return LocalRedirect(Url.Content("~/"));
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out: {Username}", Otptrans.auth.Username);
                return RedirectToAction("Lockout", "Auth");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            TempData["Otptrans"] = JsonConvert.SerializeObject(Otptrans);
            return View();
        }



        public IActionResult Lockout()
        {
            return View();
        }





        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _unitOfWork.Auth.SignOutAsync();

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToAction("Index", "Home");
            }
        }







       

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword_VM model)
            {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _unitOfWork.User.CurrentUserByUsername(model.UserId);
            
            
            if (user == null || !(await _unitOfWork.User.IsEmailConfirmedAsync(user)))
            {
                if (user == null)
                {
                    return RedirectToAction("Confirmation");
                }
                
                if (!await _unitOfWork.Auth.SendEmailOtpAsync(user.Email))
                {
                    ModelState.AddModelError(string.Empty, "Failed to send OTP. Please try again.");
                    return View(model);
                }
                TempData["Username"] = model.UserId;
                return RedirectToAction("OtpConfirmation");
            }

            var code = await _unitOfWork.User.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                action: "ResetPassword",
                controller: "Setting",
                values: new { code },
                protocol: Request.Scheme);

            await _unitOfWork.Auth.SendEmailAsync(
                user.Email,
                "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        public IActionResult OtpConfirmation()
        {

  

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OtpConfirmation(Otp_VM Otptrans)
        {
            
            if (!_unitOfWork.Auth.VerifyEmailOtp(Otptrans.InputOtp))
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired OTP. Please try again.");
                TempData["Otptrans"] = JsonConvert.SerializeObject(Otptrans);
                return View();
            }
            if (!await _unitOfWork.User.ConfirmEmailManuallyAsync(Otptrans.auth.Username))
            {
                TempData["Username"] = Otptrans.auth.Username;
                ModelState.AddModelError(string.Empty, "Unexpected Error");
                return View();
            }

            

            ForgotPassword_VM model1 = new ForgotPassword_VM
            {
                UserId = Otptrans.auth.Username


            };
            ForgotPassword(model1);
            return View("Confirmation");
        }
        public IActionResult Confirmation()
        {
            return View();
        }

    }

}

