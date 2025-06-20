using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Models.ViewModel;
using newportal.Utility;

namespace newportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Admin)]
    [Route("{area}/{controller}/{action}")]
    public class UseravailableservicesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public UseravailableservicesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult RetailerList()
        {
            return View();
        }   
        
        
        

        [HttpPost]
        public async Task<IActionResult> RetailerListJson()
        {
            // Adjust the type to match the return type of GetRetailerUserAvailableServicesAsync
            List<Useravailableservices> retailerListObjects = await _unitOfWork.userAvailableServices.GetRetailerUserAvailableServicesAsync();

            // Map the objects to the Useravailableservices type if possible
            List<Useravailableservices> retailerList = retailerListObjects
                .OfType<Useravailableservices>() // Ensure only objects of the correct type are included
                .ToList();

            return Json(new { success = true, data = retailerList });
        }


        public async Task<IActionResult> ModifyServices(string Userid)
        {
            Useravailableservices useravailableservices = await _unitOfWork.userAvailableServices.GetUserServiceByIdAsync(Userid);


            return View(useravailableservices);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifyServices(Useravailableservices obj)
        {
            bool istrue = await _unitOfWork.userAvailableServices.UpdateAsync(obj);

            if(istrue == true)
            {
                TempData["sucsmsg"] = "User services Updated Successfully";
            }
            else
            {
                TempData["errmsg"] = "User services Faild To Updated ";

            }
            return RedirectToAction("RetailerList");
        }






    }
}
