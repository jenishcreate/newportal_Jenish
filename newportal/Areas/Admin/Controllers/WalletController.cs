using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Models.ViewModel;
using newportal.Utility;
using System.Security.Claims;
using static newportal.DataAccess.Repository.WalletRepository;

namespace newportal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Admin)]
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





        public IActionResult MasterDistributor()
        {
            return View();
        }
        public async Task<JsonResult> GetAllMasterDistributor()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<ListVM> objDataList = await _unitOfWork.List.ListByParentID(userid, Role.MasterDistributor);
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
        public async Task<IActionResult> CreditAmountPost(string userId, double amount , string Tpin , string? remarks)
        {

            string initiatesuserid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var initiatesuseeData = await _unitOfWork.User.CurrentUserData(initiatesuserid);
            if (ModelState.IsValid)
            {
                var user = await _unitOfWork.User.CurrentUserData(userId);
                Wallet walletobj = await _unitOfWork.Wallet.GetUserWalletDataAsync(userId);
                if (user.ParentUserId == initiatesuserid || user.L2_ParentUserId == initiatesuserid || initiatesuserid == adminId)
            {       if(Tpin == initiatesuseeData.TPIN)
                    { 

                        
              
                            if (user != null)
                            {
                                WalletOperationResult reasult = await _unitOfWork.Wallet.CreditWalletbyadminAsync(userId, amount, initiatesuserid, "System  Credit Operation");

                                if (reasult != null)
                                {
                                    if (reasult.Success)
                                    {
                                        TempData["success"] = reasult.Message;
                                    }
                                    else
                                    {
                                        TempData["error"] = reasult.Message;
                                    }
                                }
                                else
                                {
                                    TempData["error"] = "Failed to credit amount.";

                                    TempData["success"] = "Amount credited successfully.";
                                }
                   
                   
                            }
                            else
                            {
                                TempData["error"] = "User not found.";
                            }

                    }
                    else
                    {
                        TempData["Tpinerror"] = "Incorrect Tpin";
                    }

                }
                else
                {
                    TempData["error"] = "You don't have accesss to manage this user's wallet";
                }
            }
            return RedirectToAction("Credit", new { userId = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleWalletStatus(string userId)
        {
            string initiatesuserid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _unitOfWork.User.CurrentUserData(userId);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User ID is required." });
            }

            if (user.ParentUserId == initiatesuserid || user.L2_ParentUserId == initiatesuserid || initiatesuserid == adminId)
            {
                var wallet = await _unitOfWork.Wallet.GetUserWalletDataAsync(userId);
                if (wallet == null)
                {
                    return Json(new { success = false, message = "Wallet not found." });
                }

                wallet.WalletStatus = !wallet.WalletStatus;

                _unitOfWork.Wallet.Update(wallet);
                await _unitOfWork.SaveAsync(); // ✅ this line actually commits the change to DB

                return Json(new
                {
                    success = true,
                    message = $"Wallet has been {((bool)wallet.WalletStatus ? "Unblocked" : "Blocked")} successfully.",
                    isBlocked = wallet.WalletStatus
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "You are not authorized to manage this user's wallet."
                });
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBlockedAmount(string userId, double amount)
        {
            string initiatesuserid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _unitOfWork.User.CurrentUserData(userId);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User ID is required." });
            }
            if (user.ParentUserId == initiatesuserid || user.L2_ParentUserId == initiatesuserid || initiatesuserid == adminId)
            {
                Wallet walletobj = await _unitOfWork.Wallet.GetUserWalletDataAsync(userId);
            if (walletobj == null)
            {
                return Json(new { success = false, message = "Wallet not found." });
            }

            walletobj.WalletBlockAmount = amount;
            _unitOfWork.Wallet.Update(walletobj);
                await _unitOfWork.SaveAsync();


                return Json(new
            {
                success = true,
                message = $"Blocked amount set to ₹{amount}.",
                blockedAmount = walletobj.WalletBlockAmount
            });
            }
            else
            {
                return Json(new
                {
                    success = true,
                    message = $"You Are not Authorize To Manage This User's Wallet",

                });
            }


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
        public async  Task<IActionResult> DebitAmountPost(string userId, double amount, string Tpin, string? remarks = null)
        {
            string initiatesuserid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var initiatesuseeData = await _unitOfWork.User.CurrentUserData(initiatesuserid);
            if (ModelState.IsValid)
            {
                var user = await _unitOfWork.User.CurrentUserData(userId);
                Wallet walletobj = await _unitOfWork.Wallet.GetUserWalletDataAsync(userId);

                if (user.ParentUserId == initiatesuserid || user.L2_ParentUserId == initiatesuserid || initiatesuserid == adminId)
                {
                    if (Tpin == initiatesuseeData.TPIN)
                    {
                        if (user != null)
                        {
                            WalletOperationResult reasult = _unitOfWork.Wallet.DebitWalletAsync(userId, amount, initiatesuserid, remarks).Result;

                            if (reasult != null)
                            {
                                if (reasult.Success)
                                {
                                    TempData["success"] = reasult.Message;
                                }
                                else
                                {
                                    TempData["error"] = reasult.Message;
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
                        TempData["error"] = "Incorrect Tpin";
                    }

                }
                else
                {
                    TempData["error"] = "You don't have accesss to manage this user's wallet";
                }
            }
            return RedirectToAction("Debit", new { userId = userId });


        }
    }
}