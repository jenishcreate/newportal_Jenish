using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class AddAgentController : Controller
    {


        private readonly IUnitOfWork _unitOfWork;


        public UserData_VM? userData_VM { get; set; } = new UserData_VM();

        public AddAgentController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }









        public async Task<IActionResult> AddRetailer()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            

          

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddRetailer(UserNameAndCompanyVM user)
        {

            if (!ModelState.IsValid)
            {
                return View(user);
            }
            else
            {
                string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

                string L1parentid = userid;

                var L1parentdata = await _unitOfWork.User.CurrentUserData(L1parentid);

                user.app.L2_ParentUserId = L1parentdata.ParentUserId;

                user.app.TempRole = Role.Retailer;
                await _unitOfWork.User.CreateUserByAdminAsync(user.app);

                await _unitOfWork.SaveAsync();

                return RedirectToAction("Index", "Dashboard");
            }

        }


    }
}