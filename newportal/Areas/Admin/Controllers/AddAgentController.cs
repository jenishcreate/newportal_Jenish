using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class AddAgentController : Controller
    {


        private readonly IUnitOfWork _unitOfWork;


        public UserData_VM? userData_VM { get; set; } = new UserData_VM();

        public AddAgentController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }





        public async Task<IActionResult> AddMasterDistributor()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _unitOfWork.User.parentUserCombination(userid, userid, Role.Admin);

            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> AddMasterDistributor(UserNameAndCompanyVM user)
        {

            if (!ModelState.IsValid)
            {
                string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var user1 = await _unitOfWork.User.parentUserCombination(userid, userid, Role.Admin);
                return View(user);
            }

            else
            {

                string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

                user.app.L2_ParentUserId = userid;

                user.app.ParentUserId = userid;

                user.app.TempRole = Role.MasterDistributor;

                await _unitOfWork.User.CreateUserByAdminAsync(user.app);

                await _unitOfWork.SaveAsync();

                return RedirectToAction("Index", "Dashboard");
            }

        }


        public async Task<IActionResult> AddDistributor()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _unitOfWork.User.parentUserCombination(userid, userid, Role.MasterDistributor);

            return View(user);

        }
        [HttpPost]
        public async Task<IActionResult> AddDistributor(UserNameAndCompanyVM user)
        {

            if (!ModelState.IsValid)
            {
                return View(user);
            }
            else
            {
                string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

                string L1parentid  = user.app.ParentUserId;

                var L1parentdata = await _unitOfWork.User.CurrentUserData(L1parentid);  

                user.app.L2_ParentUserId = L1parentdata.ParentUserId;
              

                user.app.TempRole = Role.Distributor;
                await _unitOfWork.User.CreateUserByAdminAsync(user.app);

                await _unitOfWork.SaveAsync();

                return RedirectToAction("Index", "Dashboard");
            }

        }






        public async Task<IActionResult> AddRetailer()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            

            var user = await _unitOfWork.User.parentUserCombination(userid, userid, Role.Distributor);

            return View(user);
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

                string L1parentid = user.app.ParentUserId;

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