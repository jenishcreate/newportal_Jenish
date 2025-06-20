using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V5.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using newportal.DataAccess.Data;
using newportal.DataAccess.Repository.Interfaces;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Utility;
using System.Data;
using System.IO;
using System.Security.Cryptography;
namespace newportal.DataAccess.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbcontext _db;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserCommissionRepository _commissionRepository;
        private readonly IUserAvailableServices _userservicess;
        private readonly IAdharpanverification _adharpanverification;
        private readonly EmailSender _emailSender;
        private readonly IWebHostEnvironment _env;
        public UserRepository(ApplicationDbcontext db, UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager, EmailSender emailSender, IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _env = env;
            _walletRepository = new WalletRepository(_db);
            _commissionRepository = new UserCommissionRepository(_db);
            _userservicess = new UserAvailableServices(_db);
            _adharpanverification = new AdharpanverificationRepository(_db);

        }


        public async Task<string> CreateUserByAdminAsync(ApplicationUser userVM)
        {
            userVM.UserName = GenerateUsernameByRole(userVM.TempRole);
            var password = GenerateSecurePassword(9) + "#";

            // Optional: ensure the NoKyc role exists
            if (!await _roleManager.RoleExistsAsync(Role.NoKyc))
            {
                await _roleManager.CreateAsync(new IdentityRole(Role.NoKyc));
            }

            // Set required properties
            await _userStore.SetUserNameAsync(userVM, userVM.UserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(userVM, userVM.Email, CancellationToken.None);

            // Create user
            var result = await _userManager.CreateAsync(userVM, password);

            if (result.Succeeded)
            {



                await _userManager.AddToRoleAsync(userVM, Role.NoKyc);


                await _walletRepository.CreateWalletAsync(userVM.Id);
                await _commissionRepository.CreateAsync(new UserCommission { ApplicationUserId = userVM.Id , Username = userVM.UserName});
                await _adharpanverification.CreateAsync( new Adharpanverification { ApplicationUserId = userVM.Id} );

                if(userVM.TempRole == Role.Retailer)
                {
                    await _userservicess.CreateAsync(new Useravailableservices { ApplicationUserId = userVM.Id , UserName = userVM.UserName });
                }
                await _emailSender.SendPasswordAsync(userVM.Email, userVM.UserName + " and password is " + password);

                return userVM.Id;
            }


            var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errorMessages}");
        }

        public async Task<ApplicationUser> CurrentUserData(string currentuserId)
        {
            ApplicationUser? userList = await _db.ApplicationUser.Where(u => u.Id == currentuserId).FirstOrDefaultAsync();


            return userList;
        }
        public async Task<IdentityUser> CurrentUser(string currentuserId)
        {

            IdentityUser? user = await _userManager.FindByIdAsync(currentuserId);


            return user;
        }
        public async Task<IdentityUser> CurrentUserByUsername(string currentuserId)
        {

            IdentityUser? user = await _userManager.FindByNameAsync(currentuserId);


            return user;
        }



        public async Task<UserNameAndCompanyVM> parentUserCombination(string userId, string parentId, string rolename)
        {
            ApplicationUser usertemp = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);

            IdentityUser usercurrent = await _userManager.FindByIdAsync(userId);

            parentId = usertemp.ParentUserId;

            var Rolenameofcurrent = await _userManager.GetRolesAsync(usercurrent);

            var usersWithRole = await (from user in _db.ApplicationUser
                                       join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                       join role in _db.Roles on userRole.RoleId equals role.Id
                                       where ((user.ParentUserId == "2cf701a1-b78f-4210-b0b6-7f7012f55145" && user.L2_ParentUserId == "2cf701a1-b78f-4210-b0b6-7f7012f55145") || (user.ParentUserId == userId || user.L2_ParentUserId == parentId || Rolenameofcurrent.Contains(Role.Admin))) && role.Name == rolename
                                       select new SelectListItem
                                       {
                                           Value = user.Id,
                                           Text = user.UserName + " - " + user.Firm_Name
                                       }).ToListAsync();

            return new UserNameAndCompanyVM
            {
                UserList = usersWithRole
            };

        }

        public async Task<UserNameAndCompanyVM> masterdistributorchind(string userId , string rolename)
        {
            ApplicationUser usertemp = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);

            IdentityUser usercurrent = await _userManager.FindByIdAsync(userId);

   

            var Rolenameofcurrent = await _userManager.GetRolesAsync(usercurrent);

            var usersWithRole = await (from user in _db.ApplicationUser
                                       join userRole in _db.UserRoles on user.Id equals userRole.UserId
                                       join role in _db.Roles on userRole.RoleId equals role.Id
                                       where ((user.ParentUserId == "2cf701a1-b78f-4210-b0b6-7f7012f55145" && user.L2_ParentUserId == "2cf701a1-b78f-4210-b0b6-7f7012f55145") || (user.ParentUserId == userId || user.L2_ParentUserId == userId || Rolenameofcurrent.Contains(Role.Admin))) && role.Name == rolename
                                       select new SelectListItem
                                       {
                                           Value = user.Id,
                                           Text = user.UserName + " - " + user.Firm_Name
                                       }).ToListAsync();

            return new UserNameAndCompanyVM
            {
                UserList = usersWithRole
            };

        }

        public string? GetProfilePictureUrlByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            var userData = _db.UserData
                .FirstOrDefault(u => u.UserDataId == userId);

            return userData?.ProfilePicture;
        }
        public string getParentId(string userId)
        {
            var parentId = _db.ApplicationUser.Where(u => u.UserName == userId).Select(u => u.ParentUserId).FirstOrDefault();

            return parentId;
        }
        public string getL2ParentId(string userId)
        {
            var parentId = _db.ApplicationUser.Where(u => u.Id == userId).Select(u => u.L2_ParentUserId).FirstOrDefault();
            return parentId;
        }


        public async Task<string> UpdateProfilePictureAsync(string userId, IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
                throw new ArgumentException("No file uploaded.");

            if (profileImage.Length > 800 * 1024)
                throw new ArgumentException("File too large.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(profileImage.ContentType))
                throw new ArgumentException("Invalid file type.");

            var user = await GetKycDetailsByUserId(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            var webRoot = _env.WebRootPath;
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profileImage.FileName)}";
            var relativePath = Path.Combine("uploads", "kyc", "Profile", fileName);
            var fullPath = Path.Combine(webRoot, relativePath);

            // Create directory if not exists
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            // Delete old image if exists
            if (!string.IsNullOrWhiteSpace(user.ProfilePicture))
            {
                var oldPath = Path.Combine(webRoot, user.ProfilePicture.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            // Save new image
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            user.ProfilePicture = "\\"+  relativePath;
            _db.UserData.Update(user);

            return user.ProfilePicture;
        }







        public async Task<bool> DeleteProfileImageAsync(string fullPathToFile)
        {
            if (System.IO.File.Exists(fullPathToFile))
            {
                try
                {
                    System.IO.File.Delete(fullPathToFile);
                    return true;
                }
                catch (Exception ex)
                {
                    // Optional: log the error
                    return false;
                }
            }
            else
            {
                return true;
            }
        }


        public async Task<UserData> GetKycDetailsByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            var model = await _db.UserData.FirstOrDefaultAsync(u => u.UserDataId == userId);
            return model;
        }




         public async Task<IdentityUser> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

        public async Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string code, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, code, newPassword);
        }








        public static string GenerateSecurePassword(int length = 12)
        {
            if (length < 3)
                throw new ArgumentException("Password length must be at least 3 to meet character requirements.");

            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string hash = "#";
            const string allChars = uppercase + lowercase + digits + hash;

            var password = new char[length];
            var rng = RandomNumberGenerator.Create();

            // Guarantee one of each required character
            password[0] = GetRandomChar(rng, uppercase);
            password[1] = GetRandomChar(rng, lowercase);
            password[2] = GetRandomChar(rng, digits);
            password[3] = GetRandomChar(rng, hash);

            // Fill remaining with random valid characters
            for (int i = 3; i < length; i++)
            {
                password[i] = GetRandomChar(rng, allChars);
            }

            // Shuffle to avoid predictable positions
            return Shuffle(password, rng);
        }

        private static char GetRandomChar(RandomNumberGenerator rng, string chars)
        {
            byte[] buffer = new byte[1];
            do
            {
                rng.GetBytes(buffer);
            } while (buffer[0] >= (byte.MaxValue - (byte.MaxValue % chars.Length))); // Remove bias

            return chars[buffer[0] % chars.Length];
        }

        private static string Shuffle(char[] array, RandomNumberGenerator rng)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                byte[] buffer = new byte[1];
                rng.GetBytes(buffer);
                int j = buffer[0] % (i + 1);

                (array[i], array[j]) = (array[j], array[i]);
            }

            return new string(array);
        }





        private string GenerateUsernameByRole(string role)
        {
            string prefix = role switch
            {
                "Retailer" => "RET",
                "Admin" => "ADM",
                "MasterDistributor" => "MAS",
                "Distributor" => "DIS",
                _ => throw new ArgumentException("Invalid role")
            };

            // Get last username with that prefix
            var lastUser = _db.Users
                .Where(u => u.UserName.StartsWith(prefix))
                .OrderByDescending(u => u.UserName)
                .FirstOrDefault();

            string lastUsername = lastUser?.UserName ?? $"{prefix}00000";
            int lastNumber = int.Parse(lastUsername.Substring(3));
            int nextNumber = lastNumber + 1;

            return $"{prefix}{nextNumber:D5}";
        }


        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }

        public async Task<Task> Logout()
        {
            await _signInManager.SignOutAsync();
            return Task.CompletedTask;
        }

        public async Task<bool> IsTwoFactorEnabledAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new InvalidOperationException("User not found.");
            return await _userManager.GetTwoFactorEnabledAsync(user);
        }

        public async Task<bool> SetTwoFactorEnabledAsync(string userId, bool isEnabled)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new InvalidOperationException("User not found.");

            var result = await _userManager.SetTwoFactorEnabledAsync(user, isEnabled);
            return result.Succeeded;
        }
        public Task<bool> IsEmailConfirmedAsync(IdentityUser user)
        {
            return _userManager.IsEmailConfirmedAsync(user);
        }
        public Task<string> GeneratePasswordResetTokenAsync(IdentityUser user)
        {
            return _userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<bool> ConfirmEmailManuallyAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return false;

            user.EmailConfirmed = true;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }


        public async Task<bool> HasPasswordAsync(IdentityUser user)
        {
            return await _userManager.HasPasswordAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(IdentityUser user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }
        public async Task RefreshSignInAsync(IdentityUser user)
        {
            await _signInManager.RefreshSignInAsync(user);
        }
        public async Task<bool> ChangeTPIN(string userId, string newTpin)
        {
            if (string.IsNullOrEmpty(userId) || newTpin == null)
                return false;
            var user = await _db.ApplicationUser.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                return false;
            user.TPIN = newTpin;
            _db.ApplicationUser.Update(user);
            int result = await _db.SaveChangesAsync();
            return result > 0;
        }
    }
}
