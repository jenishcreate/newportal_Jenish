using Microsoft.AspNetCore.Mvc;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using System.Security.Claims;

namespace newportal.Areas.NoKyc.Controllers
{

    [Area("NoKyc")]
    [Route("[area]/[controller]/[action]")]
    public class VerificationController : Controller
    {
        private readonly EmailSender _emailSender;
        private readonly PhoneSender _phoneSender;
        private readonly IUnitOfWork _unitOfWork;

        public VerificationController(EmailSender emailSender, PhoneSender phoneSender, IUnitOfWork unitOfWork)
        {
            _emailSender = emailSender;
            _phoneSender = phoneSender;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> SendOtp(string phone)
        {
            var phonetemp = "+91" + phone;
            var result = await _phoneSender.SendOtpAsync(phonetemp);
            return result ? Ok("OTP sent") : StatusCode(500, "Failed to send OTP");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpRequest model)
        {
            var result = _phoneSender.VerifyOtp(model.Phone, model.Otp);
            if (result == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Fix for CS8604: Check for null userId
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User ID is null or empty.");
                }

                // Fix for CS4032: Ensure the method is async
                Adharpanverification Verificationmodel = await _unitOfWork.adharpanverification.GetByIdAsync(userId);
                if (Verificationmodel == null)
                {
                    return NotFound("Verification model not found.");
                }

                Verificationmodel.PhoneNo_verifide = true;

                bool modelresult = await _unitOfWork.adharpanverification.UpdateAsync(Verificationmodel);
                if (!modelresult)
                {
                    return StatusCode(500, "Failed to update verification status.");
                }
            }

            return result ? Ok("OTP verified") : BadRequest("Invalid OTP");
        }

        public class OtpRequest
        {
            public string Phone { get; set; }
            public string Otp { get; set; }
        }

        public async Task<IActionResult> Send(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }
            await _emailSender.SendOtpAsync(email);
            return Ok("OTP sent.");
        }

        public async Task<IActionResult> Sendview()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Verify([FromForm] string otp)
        {
            if (string.IsNullOrEmpty(otp))
            {
                return BadRequest("Email is required.");
            }
            bool isverified = _emailSender.VerifyOtp(otp);
            if (isverified == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Fix for CS8604: Check for null userId
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User ID is null or empty.");
                }

             
                Adharpanverification Verificationmodel = await _unitOfWork.adharpanverification.GetByIdAsync(userId);
                if (Verificationmodel == null)
                {
                    return NotFound("Verification model not found.");
                }

                Verificationmodel.Email_verifide = true;

                bool modelresult = await _unitOfWork.adharpanverification.UpdateAsync(Verificationmodel);
                if (!modelresult)
                {
                    return StatusCode(500, "Failed to update verification status.");
                }
            }
            return isverified
                ? Ok("OTP verified.")
                : BadRequest("Invalid or expired OTP.");
        }
    }
}
