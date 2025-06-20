using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using newportal.DataAccess.Repository;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Models.ViewModel;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using newportal.Models.ViewModel.Auth;

namespace newportal.Controllers
{
    public class SettingController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;


        public UserData_VM? userData_VM { get; set; } = new UserData_VM();



        public SettingController( IUnitOfWork unitOfWork)
        {
            
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Profile(string Viewname = null)
        {
            UserData us = await _unitOfWork.Kyc.GetKycDetailsByUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
            TempData["User_Data"] = JsonConvert.SerializeObject(us);

            if(Viewname != null)
            {
                TempData["Viewname"] = Viewname;
            }
            return View(us);
        }


        public async Task<IActionResult> LoadPartialAsync(string viewName)
        {
            object model = null;

            if (viewName == "Profile")
            {
                UserData us = await _unitOfWork.Kyc.GetKycDetailsByUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
                TempData["User_Data"] = JsonConvert.SerializeObject(us);
            }
            else if (viewName == "ChangePassword")
            {
                model = new ChangePassword_VM();
            }
            else if (viewName == "ChangeTPIN")
            {
                // Add TPIN view model here if needed
            }
            else if (viewName == "TwoFactor")
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _unitOfWork.User.CurrentUser(userId);
                ViewBag.Is2FAEnabled = await _unitOfWork.User.IsTwoFactorEnabledAsync(userId);
            }

            return PartialView($"~/Views/_Partials/{viewName}.cshtml", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profileImage)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var imagePath = await _unitOfWork.User.UpdateProfilePictureAsync(userId, profileImage);
                await _unitOfWork.SaveAsync();
                return Json(new { success = true, imageUrl = imagePath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }



        [HttpPost]

        public async Task<IActionResult> ToggleTwoFactor(bool Enable2FA)
        {
            ToggleTwoFactorVM model = new ();
           
            

            if (model == null)
            {
                return Json(new { success = false, message = "Invalid request." });
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _unitOfWork.User.CurrentUser(userId);
            model.Enable2FA = !(await _unitOfWork.User.IsTwoFactorEnabledAsync(userId));
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var result = await _unitOfWork.User.SetTwoFactorEnabledAsync(userId, model.Enable2FA);
            if (result)
            {
                return Json(new { success = true, message = model.Enable2FA ? "Two-factor authentication enabled." : "Two-factor authentication disabled." });
            }

            return Json(new { success = false, message = "Failed to update two-factor setting." });
        }


       

        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }

            var model = new ResetPassword_VM
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))

            };

            return View(model); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPassword_VM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _unitOfWork.User.CurrentUserByUsername(model.UserId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Auth");
            }

            var result = await _unitOfWork.User.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Setting");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }



        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePassword_VM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _unitOfWork.User.CurrentUser(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _unitOfWork.User.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _unitOfWork.User.RefreshSignInAsync(user);

            TempData["StatusMessage"] = "Your password has been changed.";

            return RedirectToAction("LoadPartialAsync", new { viewName = "ChangePassword" });
        }

        [HttpPost]
        public async Task<IActionResult> SendTPINOtp()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                IdentityUser user = await _unitOfWork.User.CurrentUser(userId);
                await _unitOfWork.Auth.SendEmailOtpAsync(user.Email);
                await Task.Delay(500); 

                return Json(new { success = true, message = "OTP sent successfully." });
            }
            catch (Exception ex)
            {
                // Log error here

                return Json(new { success = false, message = "Failed to send OTP. Please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeTPIN(ChangeTPIN_VM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_unitOfWork.Auth.VerifyEmailOtp(model.OTP))
            {
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _unitOfWork.User.CurrentUser(userId);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{userId}'.");
                }
                var result = await _unitOfWork.User.ChangeTPIN(user.Id, model.NewTPIN);
                if (!result)
                {

                    ModelState.AddModelError(string.Empty, "Unexpected Error Occurred");

                    return View(model);
                }
                TempData["StatusMessage"] = "Your TPIN has been changed successfully.";
                return RedirectToAction("LoadPartialAsync", new { viewName = "ChangeTPIN" });

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP.");
                return View(model);
            }

            return View();

        }


    }
}
