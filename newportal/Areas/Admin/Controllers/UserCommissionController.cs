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
    public class UserCommissionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserCommissionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        public IActionResult MasterDistributors()
        {
            return View();
        }


        public async Task<IActionResult> MasterDistributorsJson()
        {
           
            IEnumerable<UserCommission> retailerListObjects = await _unitOfWork.userCommissionRepository.GetAllUserByRole(Role.MasterDistributor);

         
            List<UserCommission> retailerList = retailerListObjects.OfType<UserCommission>().ToList();

            return Json(new { success = true, data = retailerList });
        }


      


      

        public IActionResult Distributors()
        {
            return View();
        }

        public async Task<IActionResult> DistributorListJson()
        {
            
            IEnumerable<UserCommission> DistributorListObjects = await _unitOfWork.userCommissionRepository.GetAllUserByRole(Role.Distributor);

    
            List<UserCommission> retailerList = DistributorListObjects.OfType<UserCommission>().ToList();

            return Json(new { success = true, data = retailerList });
        }







        public IActionResult Retailers()
        {
            return View();
        }

        public async Task<IActionResult> RetailerListJson()
        {
            
            IEnumerable<UserCommission> retailerListObjects = await _unitOfWork.userCommissionRepository.GetAllUserByRole(Role.Retailer);

           
            List<UserCommission> retailerList = retailerListObjects.OfType<UserCommission>().ToList();

            return Json(new { success = true, data = retailerList });
        }




        
        [HttpGet]
        public async Task<IActionResult> EditCommission(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var userCommission = await _unitOfWork.userCommissionRepository.GetByIdAsync(userId);
            if (userCommission == null)
                return NotFound();

            return View(userCommission);
        }

        // POST: Update UserCommission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCommission(UserCommission model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _unitOfWork.userCommissionRepository.UpdateAsync(model);
            if (!result)
            {
                ModelState.AddModelError("", "Failed to update commission.");
                return View(model);
            }

            TempData["success"] = "Commission updated successfully!";
            return RedirectToAction("MasterDistributors"); // or Retailers/Distributors as per context
        }


    }
}
