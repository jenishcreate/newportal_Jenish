using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace newportal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
      
        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            
            return View();
        }



        public async Task<IActionResult> AddTempAdmin()
        {
            

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddTempAdminPost(UserNameAndCompanyVM user)
        {



            

                user.app.TempRole = Role.Admin;

                await _unitOfWork.User.CreateUserByAdminAsync(user.app);

                await _unitOfWork.SaveAsync();

                return RedirectToAction("Index", "Dashboard");
            

        }


        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
