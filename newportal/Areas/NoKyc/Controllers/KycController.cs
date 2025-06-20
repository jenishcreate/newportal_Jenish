using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Models.ViewModel;
using newportal.Utility;
using System.Security.Claims;
namespace newportal.Areas.NoKyc.Controllers
{
    [Area("NoKyc")]
    [Route("{area}/{controller}/{action}")]
    [Authorize(Roles = Role.NoKyc)]
    public class KycController : Controller
    {

        private readonly IWebHostEnvironment _env;
        private readonly IUnitOfWork _unitOfWork;


        public UserData_VM? userData_VM { get; set; } = new UserData_VM();



        public KycController(IWebHostEnvironment env, IUnitOfWork unitOfWork)
        {
            _env = env;
            _unitOfWork = unitOfWork;
        }











        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = await _unitOfWork.User.CurrentUserData(userId);


            try
            {
                UserData Kycdata = _unitOfWork.Kyc.GetKycDetailsByUserId(userId).Result;

                if (Kycdata != null)
                {
                    if (Kycdata.KYC_Submitted == true && Kycdata.Status == KycStatus.Rejected.ToString())
                    {
                        return RedirectToAction("EditKyc");
                    }
                    if (Kycdata.KYC_Submitted == true && Kycdata.Status == KycStatus.Pending.ToString())
                    {
                        return RedirectToAction("KycSubmited");
                    }

                }
            }
            catch (Exception ex)
            {

                NotFound();

            }

            if (user == null)
            {
                return NotFound();
            }
            ViewBag.email = user.Email;
            ViewBag.mobile = user.PhoneNumber;
            ViewBag.whatsAppNo = user.WhatsAppPhoneNumber;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyKyc(UserData_VM model)
        {
            if (!ModelState.IsValid)
            {
                return View("RegisterUser", model);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = await _unitOfWork.User.CurrentUserData(userId);

            if (user == null)
            {
                return NotFound();
            }
            model.UserData.Companyname = user.Firm_Name;

            model.UserData.UserDataId = userId;

            string folderpathAdhar = Path.Combine("uploads", "kyc", "AdharCard");
            string folderpathPan = Path.Combine("uploads", "kyc", "PanCard");
            string folderpathCheque = Path.Combine("uploads", "kyc", "Cheque");
            string folderpathProfile = Path.Combine("uploads", "kyc", "Profile");


            string uploadPathAdhar = Path.Combine(_env.WebRootPath, folderpathAdhar);
            string uploadPathpan = Path.Combine(_env.WebRootPath, folderpathPan);
            string uploadPathcheque = Path.Combine(_env.WebRootPath, folderpathCheque);
            string uploadPathprofile = Path.Combine(_env.WebRootPath, folderpathProfile);
            Directory.CreateDirectory(uploadPathAdhar);
            Directory.CreateDirectory(uploadPathpan);
            Directory.CreateDirectory(uploadPathcheque);
            Directory.CreateDirectory(uploadPathprofile);


            // Save Aadhaar Front
            if (model.AadhaarFront != null)
            {


                string fileName = Path.Combine(Guid.NewGuid() + Path.GetExtension(model.AadhaarFront.FileName));

                string filePath = Path.Combine(uploadPathAdhar, fileName);

                string Storefile = "\\" + Path.Combine(folderpathAdhar, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await model.AadhaarFront.CopyToAsync(stream);
                {
                    model.UserData.Adhar_Front_Photo_Url = Storefile;
                }
                stream.Close();

            }

            // Save Aadhaar Back
            if (model.AadhaarBack != null)
            {
                string fileName = Path.Combine(Guid.NewGuid() + Path.GetExtension(model.AadhaarBack.FileName));

                string filePath = Path.Combine(uploadPathAdhar, fileName);

                string Storefile = "\\" + Path.Combine(folderpathAdhar, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.AadhaarBack.CopyToAsync(stream);
                {
                    model.UserData.Adhar_Back_Photo_Url = Storefile;
                }
                stream.Close();
            }

            // Save PAN Image
            if (model.PanImage != null)
            {
                string fileName = Path.Combine(Guid.NewGuid() + Path.GetExtension(model.PanImage.FileName));

                string filePath = Path.Combine(uploadPathpan, fileName);

                string Storefile = "\\" + Path.Combine(folderpathPan, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await model.PanImage.CopyToAsync(stream);
                {
                    model.UserData.Pan_Photo_Url = Storefile;
                }
                stream.Close();
            }

            if (model.ProfilePicture != null)
            {
                string fileName = Path.Combine(Guid.NewGuid() + Path.GetExtension(model.ProfilePicture.FileName));

                string filePath = Path.Combine(uploadPathprofile, fileName);

                string Storefile = "\\" + Path.Combine(folderpathProfile, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await model.ProfilePicture.CopyToAsync(stream);
                {
                    model.UserData.ProfilePicture = Storefile;
                }
                stream.Close();
            }

            if (model.Blankcheque != null)
            {
                string fileName = Path.Combine(Guid.NewGuid() + Path.GetExtension(model.Blankcheque.FileName));

                string filePath = Path.Combine(uploadPathcheque, fileName);

                string Storefile = "\\" + Path.Combine(folderpathCheque, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await model.Blankcheque.CopyToAsync(stream);
                {
                    model.UserData.Blank_Check_Photo_Url = Storefile;
                }
                stream.Close();
            }



            model.UserData.KYC_Submitted = true;
            if (model.PanImage != null && model.AadhaarBack != null && model.AadhaarFront != null)
            {
                await _unitOfWork.Kyc.SubmitKycAsync(model);
                await _unitOfWork.SaveAsync();
            }




            TempData["Success"] = "KYC Submitted Successfully!";
            return View("KycSubmited");
        }




        public IActionResult KycSubmited()
        {
            return View();
        }



        [HttpGet]
        public async Task<IActionResult> EditKyc()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userData = await _unitOfWork.Kyc.GetKycDetailsByUserId(userId);
            ApplicationUser user = await _unitOfWork.User.CurrentUserData(userId);
            ViewBag.email = user.Email;
            ViewBag.mobile = user.PhoneNumber;
            ViewBag.whatsAppNo = user.WhatsAppPhoneNumber;

            if (userData == null)
                return NotFound();

            var viewModel = new UserData_VM
            {
                UserData = userData
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditKyc(UserData_VM model,
            bool RemoveAdharFront, bool RemoveAdharBack, bool RemovePan, bool RemoveProfile, bool RemoveCheque)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existing = await _unitOfWork.Kyc.GetKycDetailsByUserId(userId);
            if (existing == null)
                return NotFound();

            string root = _env.WebRootPath;

            string pathAdhar = Path.Combine(root, "uploads", "kyc", "AdharCard");
            string pathPan = Path.Combine(root, "uploads", "kyc", "PanCard");
            string pathCheque = Path.Combine(root, "uploads", "kyc", "Cheque");
            string pathProfile = Path.Combine(root, "uploads", "kyc", "Profile");

            Directory.CreateDirectory(pathAdhar);
            Directory.CreateDirectory(pathPan);
            Directory.CreateDirectory(pathCheque);
            Directory.CreateDirectory(pathProfile);

            void DeleteIfExists(string relativePath)
            {
                if (!string.IsNullOrEmpty(relativePath))
                {
                    string full = Path.Combine(root, relativePath.TrimStart('\\'));
                    if (System.IO.File.Exists(full))
                        System.IO.File.Delete(full);
                }
            }

            // Aadhaar Front
            if (RemoveAdharFront)
            {
                DeleteIfExists(existing.Adhar_Front_Photo_Url);
                existing.Adhar_Front_Photo_Url = null;
            }
            else if (model.AadhaarFront != null && model.AadhaarFront.Length > 0)
            {
                DeleteIfExists(existing.Adhar_Front_Photo_Url);
                string fileName = Guid.NewGuid() + Path.GetExtension(model.AadhaarFront.FileName);
                string filePath = Path.Combine(pathAdhar, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.AadhaarFront.CopyToAsync(stream);
                existing.Adhar_Front_Photo_Url = "\\uploads\\kyc\\AdharCard\\" + fileName;
            }

            // Aadhaar Back
            if (RemoveAdharBack)
            {
                DeleteIfExists(existing.Adhar_Back_Photo_Url);
                existing.Adhar_Back_Photo_Url = null;
            }
            else if (model.AadhaarBack != null && model.AadhaarBack.Length > 0)
            {
                DeleteIfExists(existing.Adhar_Back_Photo_Url);
                string fileName = Guid.NewGuid() + Path.GetExtension(model.AadhaarBack.FileName);
                string filePath = Path.Combine(pathAdhar, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.AadhaarBack.CopyToAsync(stream);
                existing.Adhar_Back_Photo_Url = "\\uploads\\kyc\\AdharCard\\" + fileName;
            }

            // PAN
            if (RemovePan)
            {
                DeleteIfExists(existing.Pan_Photo_Url);
                existing.Pan_Photo_Url = null;
            }
            else if (model.PanImage != null && model.PanImage.Length > 0)
            {
                DeleteIfExists(existing.Pan_Photo_Url);
                string fileName = Guid.NewGuid() + Path.GetExtension(model.PanImage.FileName);
                string filePath = Path.Combine(pathPan, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.PanImage.CopyToAsync(stream);
                existing.Pan_Photo_Url = "\\uploads\\kyc\\PanCard\\" + fileName;
            }

            // Profile
            if (RemoveProfile)
            {
                DeleteIfExists(existing.ProfilePicture);
                existing.ProfilePicture = null;
            }
            else if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                DeleteIfExists(existing.ProfilePicture);
                string fileName = Guid.NewGuid() + Path.GetExtension(model.ProfilePicture.FileName);
                string filePath = Path.Combine(pathProfile, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.ProfilePicture.CopyToAsync(stream);
                existing.ProfilePicture = "\\uploads\\kyc\\Profile\\" + fileName;
            }

            // Cheque
            if (RemoveCheque)
            {
                DeleteIfExists(existing.Blank_Check_Photo_Url);
                existing.Blank_Check_Photo_Url = null;
            }
            else if (model.Blankcheque != null && model.Blankcheque.Length > 0)
            {
                DeleteIfExists(existing.Blank_Check_Photo_Url);
                string fileName = Guid.NewGuid() + Path.GetExtension(model.Blankcheque.FileName);
                string filePath = Path.Combine(pathCheque, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.Blankcheque.CopyToAsync(stream);
                existing.Blank_Check_Photo_Url = "\\uploads\\kyc\\Cheque\\" + fileName;
            }

            // Basic Info Update
            existing.FirstName = model.UserData.FirstName;
            existing.MiddleName = model.UserData.MiddleName;
            existing.LastName = model.UserData.LastName;
            existing.DOB = model.UserData.DOB;
            existing.Gender = model.UserData.Gender;
            existing.Adhar_No = model.UserData.Adhar_No;
            existing.Pan_No = model.UserData.Pan_No;
            existing.Address = model.UserData.Address;
            existing.City = model.UserData.City;
            existing.State = model.UserData.State;
            existing.Country = model.UserData.Country;
            existing.PinCode = model.UserData.PinCode;
            existing.Companyname = model.UserData.Companyname;
            existing.Udhyam_Adhar_NO = model.UserData.Udhyam_Adhar_NO;
            existing.GST_NO = model.UserData.GST_NO;

            existing.KYC_Submitted = true;
            existing.Status = KycStatus.Pending.ToString();

            await _unitOfWork.SaveAsync();
            TempData["Success"] = "KYC updated successfully!";
            return RedirectToAction(nameof(KycSubmited));
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folderRelative)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string saveFolder = Path.Combine(_env.WebRootPath, folderRelative);
            Directory.CreateDirectory(saveFolder); // Ensure folder exists

            string fullPath = Path.Combine(saveFolder, fileName);
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName;
        }



        public IActionResult KycDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserData Kycdata = _unitOfWork.Kyc.GetKycDetailsByUserId(userId).Result;

            return View(Kycdata);
        }


        [HttpPost]
        public async Task<IActionResult> signout()
        {
            await _unitOfWork.User.Logout();


            return RedirectToAction("Index", "Home");
        }


    }
}

