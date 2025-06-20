using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using newportal.Models.ViewModel;
using newportal.Models;
using newportal.Utility;
using System.Text.Json;
using newportal.DataAccess.Repository.IRepository;
using static newportal.DataAccess.Repository.WalletRepository;

namespace newportal.Areas.Retailer.Controllers
{
    [Area("Retailer")]
    [Authorize(Roles = Role.Retailer)]
    [Route("{area}/{controller}/{action}")]
    public class RechargeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RechargeController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }
        public IActionResult MakeRecharge()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeRecharge(Recharge_VM model)
        {
            
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();


            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["msg"] = "User not authenticated.";
                return View(model);
            }
            if (!Request.Form.ContainsKey("Latitude") || !Request.Form.ContainsKey("Longitude"))
            {
                TempData["msg"] = "Location data is required to proceed.";
                return View(model); // Or redirect back
            }

            if (!double.TryParse(Request.Form["Latitude"], out var latitude) ||
                !double.TryParse(Request.Form["Longitude"], out var longitude))
            {
                TempData["msg"] = "Please Enable the Location Permision And Reload The Browser, Location data is required to proceed.";
                return View(model);
            }

            // Validate coordinates range
            if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
            {
                TempData["msg"] = "Location coordinates are out of bounds.";
                return View(model);
            }

            Useravailableservices service = await _unitOfWork.userAvailableServices.GetUserServiceByIdAsync(currentUserId);
            if(service.RechargeService == false)
            {
                TempData["msg"] = "You Are Not Allow To Use This Service Or Your Service Is Bloclked, Kindly Contact Your Parent User";
                return View(model);

            }
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Please fill all required fields correctly.";
                return View(model);
            }

          
            var user = await _unitOfWork.User.CurrentUserData(currentUserId);

            if (user == null)
            {
                TempData["msg"] = "User not found.";
                return View(model);
            }   


            if (user.TPIN != model.Tpin)
            {
                TempData["msg"] = "Invalid TPIN.";
                return View(model);
            }
            if (model.RechargeAmount <= 0)
            {
                TempData["msg"] = "Recharge amount must be greater than zero.";
                return View(model);
            }
           

            bool isavailablebalance = await _unitOfWork.Wallet.Checkuserbalanceforservice(currentUserId, (double)model.RechargeAmount);
            if (!isavailablebalance)
            {
                TempData["msg"] = "Insufficient balance in wallet.";
                return View(model);
            }

            var parentuser = await _unitOfWork.User.CurrentUserData(user.ParentUserId);
            var L2parentuser = await _unitOfWork.User.CurrentUserData(user.L2_ParentUserId);

            Wallet walletdata = await _unitOfWork.Wallet.GetUserWalletDataAsync(currentUserId);

            if (model.RechargeType == "Prepaid")
            {
                var transaction = new RechargeTransaction
                {
                    Initiated_UserId = user.Id,
                    Initiated_UserName = user.UserName,
                    Initiated_User_IP = clientIp,

                    User_Latitude= latitude,
                    User_Longitude = longitude,

                    Initiated_User_number = user.PhoneNumber,
                    Customer_MobileNumber = model.Customer_MobileNumber,
                    Initiated_companyname = user.Firm_Name,
                    Initiated_User_Parentid = user.ParentUserId,
                    Initiated_User_Parent_Number = parentuser.PhoneNumber,
                    Initiated_User_ParentCompany = parentuser.Firm_Name,
                    Initiated_User_L2_Parent_Number = L2parentuser.PhoneNumber,
                    Initiated_User_L2_ParentCompany = L2parentuser.Firm_Name,
                    Initiated_User_L2_Parentid = user.L2_ParentUserId,
                    MobileNumber_operator = model.MobileNumber_operator,
                    Circle = model.Circle,
                    RechargeType = model.RechargeType,
                    RechargeAmount = model.RechargeAmount,
                    debited_Amount = model.RechargeAmount,
                    Last_updatedbalance = (decimal)walletdata.WalletBalance, 
                    Status = "Initiated",
                    TransactionDate = DateTime.Now
                };

                bool res = await _unitOfWork.Rechargetransaction.AddRechargeTransactionAsync(transaction);
                if (res == true)
                {
                    WalletOperationResult updateWallet = await _unitOfWork.Wallet.RecordServiceUsageAsync(
                        currentUserId,
                        (double)model.RechargeAmount, 
                        transaction.Recharge_transaction_id,
                        $"{model.RechargeAmount} Amount Deducted for {model.RechargeType} Recharge",
                        "Recharge Service Debit"
                    );
                }

                TempData["Success"] = "Recharge initiated successfully!";
            }
            else if (model.RechargeType == "Postpaid")
            {
                TempData["Info"] = "Postpaid bill fetch initiated. You will now confirm the payment.";
                var transaction = new RechargeTransaction
                {
                    Initiated_UserId = user.Id,
                    Initiated_UserName = user.UserName,
                    Initiated_User_IP = clientIp,

                    User_Latitude = latitude,
                    User_Longitude = latitude,

                    Initiated_User_number = user.PhoneNumber,
                    Customer_MobileNumber = model.Customer_MobileNumber,
                    Initiated_companyname = user.Firm_Name,
                    Initiated_User_Parentid = user.ParentUserId,
                    Initiated_User_Parent_Number = parentuser.PhoneNumber,
                    Initiated_User_ParentCompany = parentuser.Firm_Name,
                    Initiated_User_L2_Parent_Number = L2parentuser.PhoneNumber,
                    Initiated_User_L2_ParentCompany = L2parentuser.Firm_Name,
                    Initiated_User_L2_Parentid = user.L2_ParentUserId,
                    MobileNumber_operator = model.MobileNumber_operator,
                    Circle = model.Circle,
                    RechargeType = model.RechargeType,
                    RechargeAmount = model.RechargeAmount,
                    debited_Amount = model.RechargeAmount,
                    Last_updatedbalance = (decimal)walletdata.WalletBalance,
                    Status = "Initiated",
                    TransactionDate = DateTime.Now
                };

                bool res = await _unitOfWork.Rechargetransaction.AddRechargeTransactionAsync(transaction);
                if (res == true)
                {
                    WalletOperationResult updateWallet = await _unitOfWork.Wallet.RecordServiceUsageAsync(
                        currentUserId,
                        (double)model.RechargeAmount,
                        transaction.Recharge_transaction_id,
                        $"{model.RechargeAmount} Amount Deducted for {model.RechargeType} Recharge",
                        "Recharge Service Debit"
                    );
                }

                TempData["Success"] = "Recharge initiated successfully!";
            }

            return RedirectToAction("MakeRecharge");
        }

    }
   

}
