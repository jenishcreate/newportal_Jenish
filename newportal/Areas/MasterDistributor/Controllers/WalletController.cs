using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Models.ViewModel;
using newportal.Utility;
using System.Security.Claims;
using static newportal.DataAccess.Repository.WalletRepository;

namespace newportal.Areas.MasterDistributor.Controllers
{
    [Area("MasterDistributor")]
    [Authorize(Roles = Role.MasterDistributor)]
    [Route("{area}/{controller}/{action}")]
    public class WalletController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string adminId;
        public WalletController(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            adminId = config["Admin:Id"];
        }


        public IActionResult RetailerList()

        {
            return View();
        }
        public async Task<JsonResult> GetAllRetailer()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<ListVM> objDataList = await _unitOfWork.List.ListByParentID(userid, Role.Retailer);

            return Json(new { data = objDataList });
        }




        public IActionResult Distributor()
        {
            return View();
        }
        public async Task<JsonResult> GetAllDistributor()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<ListVM> objDataList = await _unitOfWork.List.ListByParentID(userid, Role.Distributor);
            return Json(new { data = objDataList });
        }





      



        public async Task<IActionResult> Credit(string userId)
        {
            var userobj = await _unitOfWork.User.CurrentUserData(userId);
            Wallet walletobj = await _unitOfWork.Wallet.GetUserWalletDataAsync(userId);
            UserData addobj = await _unitOfWork.Kyc.GetKycDetailsByUserId(userId);
            ViewBag.UserId = userId;
            ViewBag.UserName = userobj.UserName;
            ViewBag.CompanyName = userobj.Firm_Name;
            ViewBag.MobileNo = userobj.PhoneNumber;
            ViewBag.UserType = userobj.TempRole;
            ViewBag.EmailId = userobj.Email;
            ViewBag.WalletBalance = walletobj.WalletBalance;
            if (addobj == null)
            {
                ViewBag.Address = "not submited";
            }
            else
            {
                ViewBag.Address = addobj.Address + ", " + addobj.City + ", " + addobj.State;
            }

            ViewBag.ParentId = userobj.ParentUserId;

            ViewBag.Status = userobj.IsActive;
            return View();

        }



        [HttpPost]
        public async Task<IActionResult> CreditAmountPost(string touserId, double amount)
        {

            string initiatesuserid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                var user = await _unitOfWork.User.CurrentUserData(touserId);
                Wallet walletobj = await _unitOfWork.Wallet.GetUserWalletDataAsync(touserId);
                if (user.ParentUserId == initiatesuserid || user.L2_ParentUserId == initiatesuserid || initiatesuserid == adminId)
                {
              
                    if (user != null)
                    {
                        WalletOperationResult reasult = await _unitOfWork.Wallet.TransferWalletAsync(initiatesuserid, touserId, amount, initiatesuserid,"");

                        if (reasult != null)
                        {
                            if (reasult.Success == true)
                            {
                                TempData["fundsuccess"] = reasult.Message;
                            }
                            else
                            {
                                TempData["Funderror"] = reasult.Message;
                            }
                        }
                    }
                
                }
                else
                {
                    TempData["error"] = "You don't have accesss to manage this user's wallet";
                }
            }
            return RedirectToAction("Credit", new { userId = touserId });
        }

        public async Task<IActionResult> Debit(string userId)
        {
            var userobj = await _unitOfWork.User.CurrentUserData(userId);
            Wallet walletobj = await _unitOfWork.Wallet.GetUserWalletDataAsync(userId);
            UserData addobj = await _unitOfWork.Kyc.GetKycDetailsByUserId(userId);
            ViewBag.UserId = userId;
            ViewBag.UserName = userobj.UserName;
            ViewBag.CompanyName = userobj.Firm_Name;
            ViewBag.MobileNo = userobj.PhoneNumber;
            ViewBag.UserType = userobj.TempRole;
            ViewBag.EmailId = userobj.Email;
            ViewBag.WalletBalance = walletobj.WalletBalance;
            if( addobj == null )
            {
                ViewBag.Address = "not submited";
            }
            else
            {
                ViewBag.Address = addobj.Address + ", " + addobj.City + ", " + addobj.State;
            }
              
            ViewBag.ParentId = userobj.ParentUserId;
          
            ViewBag.Status = userobj.IsActive;

            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> DebitAmountPost(string fromuserId, double amount, string? remarks = null)
        {
            string initiatesuserid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                var user = await _unitOfWork.User.CurrentUserData(fromuserId);
                Wallet walletobj = await _unitOfWork.Wallet.GetUserWalletDataAsync(fromuserId);

                if (user.ParentUserId == initiatesuserid || user.L2_ParentUserId == initiatesuserid || initiatesuserid == adminId)
                {
                        if (user != null)
                        {
                        WalletOperationResult reasult = _unitOfWork.Wallet.TransferWalletAsync(fromuserId, initiatesuserid, amount, initiatesuserid, remarks).Result;

                        if (reasult != null)
                        {
                            if (reasult.Success == true)
                            {
                                TempData["fundsuccess"] = reasult.Message;
                            }
                            else
                            {
                                TempData["Funderror"] = reasult.Message;
                            }
                        }
                        else
                        {
                            TempData["error"] = "Failed to debit amount.";
                        }

                    
                        }
                        else
                        {
                            TempData["error"] = "User not found.";
                        }
                }
                else
                {
                    TempData["error"] = "You don't have accesss to manage this user's wallet";
                }
            }
            return RedirectToAction("Debit", new { userId = fromuserId });


        }
    }
}