using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Models.ViewModel;
using newportal.Utility;

namespace newportal.Areas.Distributor.Controllers
{
    [Area("Distributor")]
    [Authorize(Roles = Role.Distributor)]
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

        [HttpPost]
        public async Task<JsonResult> WalletDistributorTransacctionsJson(DateTime startDate, DateTime endDate)
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<WalletTransaction> objDataList = await _unitOfWork.Wallet.GetWalletHistoryAsync(userid, startDate, endDate);


            return Json(new { data = objDataList });
        }

    }
}
