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
    public class ListController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IActionResult> ListRetailerAsync()
        {

            return View();
        }






        public async Task<JsonResult> GetAllRetailer()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<ListVM> objDataList = await _unitOfWork.List.ListByParentID(userid, Role.Retailer);

            return Json(new { data = objDataList });
        }











        public IActionResult ListMasterDistributor()
        {


            return View();
        }


        [HttpPost]
        public async Task<JsonResult> GetAllMasterDistributor()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<ListVM> objDataList = await _unitOfWork.List.ListByParentID(userid, Role.MasterDistributor);

            return Json(new { data = objDataList });
        }









        public IActionResult ListDistributor()
        {


            return View();
        }


        [HttpPost]
        public async Task<JsonResult> GetAllDistributor()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<ListVM> objDataList = await _unitOfWork.List.ListByParentID(userid, Role.Distributor);

            return Json(new { data = objDataList });
        }











    }
}
