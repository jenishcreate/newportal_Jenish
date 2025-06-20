using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models.ViewModel;
using newportal.Utility;
using System.Security.Claims;

namespace newportal.Areas.Admin.Controllers
{

    [Area("Admin")]
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





        [HttpGet]
        public IActionResult Rolemanagement()
        {
            RoleManagementVM = new()
            {
                Rolelist = _unitOfWork.Role.GetAllRoles()
            };

            return View(RoleManagementVM);
        }


        [HttpPost]
        public async Task<IActionResult> RolemanagementAsync(bool temp = true)
        {

            if (_unitOfWork.Role.Check(Role.MasterDistributor))
            {
                await _unitOfWork.Role.CreateAsync(Role.Admin);
                await _unitOfWork.Role.CreateAsync(Role.MasterDistributor);
                await _unitOfWork.Role.CreateAsync(Role.Distributor);
                await _unitOfWork.Role.CreateAsync(Role.Retailer);
                await _unitOfWork.Role.CreateAsync(Role.NoKyc);
                await _unitOfWork.Role.CreateAsync(Role.SubAdmin);
                await _unitOfWork.SaveAsync();

            }
            RoleManagementVM = new()
            {
                Rolelist = _unitOfWork.Role.GetAllRoles()
            };
            return View(RoleManagementVM);
        }


    }
}
