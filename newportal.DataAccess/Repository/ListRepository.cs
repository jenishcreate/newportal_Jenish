using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using newportal.DataAccess.Data;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models.ViewModel;
using newportal.Utility;

namespace newportal.DataAccess.Repository
{
    public class ListRepository : IListRepository
    {
        private readonly ApplicationDbcontext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
  
        private readonly string adminId;
        public ListRepository(ApplicationDbcontext db, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            adminId = config["Admin:Id"];
        }
        public async Task<List<ListVM>> ListByParentID(string parentId, string roleName)
        {

            var userList = await (
                from user in _db.ApplicationUser
                join userRole in _db.UserRoles on user.Id equals userRole.UserId
                join role in _db.Roles on userRole.RoleId equals role.Id
                where (user.ParentUserId == parentId || user.L2_ParentUserId == parentId || parentId == adminId ? true:false) && user.TempRole == roleName
                join wallet in _db.wallet on user.Id equals wallet.UserId into walletGroup
                from wallet in walletGroup.DefaultIfEmpty()
                join userData in _db.UserData on user.Id equals userData.UserDataId into userDataGroup
                from userData in userDataGroup.DefaultIfEmpty()
                select new ListVM
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    CompanyName = userData.Companyname ?? user.Firm_Name,
                    MobileNo = user.PhoneNumber,
                    UserType = user.TempRole ?? role.Name,
                    EmailId = user.Email,
                    WalletBalance = (decimal)(wallet != null ? wallet.WalletBalance ?? 0 : 0),
                    Blockedamount = (decimal)(wallet != null ? wallet.WalletBlockAmount ?? 0 : 0),
                    Walletstatus = wallet != null && wallet.WalletStatus.HasValue ? wallet.WalletStatus.Value : false,
                    JoinDate = user.CreatedDate.ToString("yyyy-MM-dd"),
                    ParentId = _db.ApplicationUser.Where(u => u.Id == user.ParentUserId).Select(u => u.UserName).FirstOrDefault(),
                    KYCStatus = userData != null ? userData.Status : (user.KycStatus ? "Approved" : "Pending"),
                    Status = user.IsActive ? "Active" : "Inactive",
                }
            ).ToListAsync();

            
            return userList;
        }


        public async Task<List<ListVM>> GetUsernameAndCompanyNameByIdRole(string parentId, string roleName)
        {
            var userList = await (
                from user in _db.ApplicationUser
                join userRole in _db.UserRoles on user.Id equals userRole.UserId
                join role in _db.Roles on userRole.RoleId equals role.Id
                where user.ParentUserId == parentId && user.TempRole == roleName
                select new ListVM
                {

                    UserName = user.Id,
                    CompanyName = user.Firm_Name


                }
            ).ToListAsync();

            return userList;
        }




    }
}
