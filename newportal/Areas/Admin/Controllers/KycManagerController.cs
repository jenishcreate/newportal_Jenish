using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Utility;

namespace newportal.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = Role.Admin)]
    [Route("{area}/{controller}/{action}")]
    public class KycManagerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public KycManagerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult KycList()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Approve(string userId)
        {
            var ok = await _unitOfWork.Kyc.ApproveKycAsync(userId);
            TempData[ok ? "Success" : "Error"] = ok
                ? "KYC approved."
                : "Could not approve.";
            return RedirectToAction(nameof(KycList));
        }

        public async Task<IActionResult> KycDetails(string userId)
        {
            UserData Kycdata = await _unitOfWork.Kyc.GetKycDetailsByUserId(userId);
            return View(Kycdata);
        }

       
        public async Task<IActionResult> Adhaparnverification(string userId)
        {
            Adharpanverification Verificationmodel = await _unitOfWork.adharpanverification.GetByIdAsync(userId);
            return Json(new {
                success = true,
                adharVerified = Verificationmodel.Adhar_verifide,
                panVerified = Verificationmodel.Pan_verifide,
                phoneVerified = Verificationmodel.PhoneNo_verifide,
                emailVerified = Verificationmodel.Email_verifide,
                whatsappVerified = Verificationmodel.WhatsappNo_verifide
            });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(string userId, string remarks)
        {
            var ok = await _unitOfWork.Kyc.RejectKycAsync(userId, remarks);
            TempData[ok ? "Success" : "Error"] = ok
                ? "KYC rejected."
                : "Could not reject.";
            return RedirectToAction(nameof(KycList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAll()
        {
            List<UserData> objDataList = await _unitOfWork.Kyc.GellAllKycoList();
            return Json(new { data = objDataList });
        }
    }
}
