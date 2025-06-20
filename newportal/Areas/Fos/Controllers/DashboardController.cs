using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models.ViewModel;
using newportal.Utility;
using System.Security.Claims;

namespace newportal.Areas.Fos.Controllers
{

    [Area("Fos")]
    [Authorize(Roles = Role.Admin)]
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
        public async Task<IActionResult> TotalBalance()
        {
            var balance = await _unitOfWork.Wallet.GetTotalSystemBalanceAsync();
            return Json(new { balance });
        }






    }
}
