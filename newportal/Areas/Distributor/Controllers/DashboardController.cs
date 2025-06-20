using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models.ViewModel;
using newportal.Utility;
using System.Security.Claims;

namespace newportal.Areas.Distributor.Controllers
{

    [Area("Distributor")]
    [Authorize(Roles = Role.Distributor)]
    [Route("{area}/{controller}/{action}")]
    public class DashboardController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public RoleManagementVM? RoleManagementVM { get; set; } = new RoleManagementVM();
        public UserData_VM? userData_VM { get; set; } = new UserData_VM();


        public DashboardController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }



        public IActionResult Index()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            ViewData["profile"] = _unitOfWork.User.GetProfilePictureUrlByUserId(userid);
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetdistributorBalance()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var balance = await _unitOfWork.Wallet.GetUserWalletBalanceAsync(userid);
            return Json(new { balance });
        }
        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userinfo = await _unitOfWork.User.CurrentUserData(userid);
            return Json(new { userName = userinfo.UserName, companyName = userinfo.Firm_Name });
        }



    }
}
