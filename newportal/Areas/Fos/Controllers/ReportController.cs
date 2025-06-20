using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Models.ViewModel;
using newportal.Utility;

namespace newportal.Areas.Fos.Controllers
{
    [Area("Fos")]
    [Authorize(Roles = Role.Admin)]
    [Route("{area}/{controller}/{action}")]
    public class ReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult WalletTransacctions()
        {
            return View();
        }
        
        public async Task<JsonResult> WalletTransacctionsJson(DateTime startDate, DateTime endDate)
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<WalletTransaction> objDataList = await _unitOfWork.Wallet.GetWalletHistoryAsync(userid, startDate, endDate);


            return Json(new { data = objDataList });
        }

    }
}
