using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using newportal.DataAccess.Data;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Models.ViewModel;
using newportal.Utility;

namespace newportal.DataAccess.Repository
{
    public class KycRepository : Repository<UserData>, IKycRepository
    {
        private readonly ApplicationDbcontext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;



        public KycRepository(ApplicationDbcontext db, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager) : base(db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }



        public Task<bool> SubmitKycAsync(UserData_VM model)
        {
            _db.UserData.Add(model.UserData);
            return Task.FromResult(true);
        }


        public async Task<UserData> GetKycDetailsByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            var model = await _db.UserData.FirstOrDefaultAsync(u => u.UserDataId == userId);
            return model;
        }



        public async Task<List<UserData>> GellAllKycoList()
        {
            return await _db.UserData
        .Where(ud => ud.Status == KycStatus.Pending.ToString())
        .ToListAsync();
        }




        public async Task<bool> UpdateKycAsync(UserData userData)
        {
            var existingData = await _db.UserData.FirstOrDefaultAsync(u => u.UserDataId == userData.UserDataId);
            if (existingData == null)
                return false;

            _db.Entry(existingData).CurrentValues.SetValues(userData);
            return true;
        }




        public async Task<bool> ApproveKycAsync(string userId)
        {
            var kyc = await _db.UserData.FirstOrDefaultAsync(u => u.UserDataId == userId);
            var user = await _db.ApplicationUser.FirstOrDefaultAsync(_db => _db.Id == userId);


            if (kyc == null || user == null)
            {
                return false;
            }

            kyc.Status = KycStatus.Approved.ToString();
            kyc.AdminRemarks = null;
            user.KycStatus = true;
            user.KycApproveConfirmed = true;

            // only remove the NoKyc role
            if (await _userManager.IsInRoleAsync(user, Role.NoKyc))
                await _userManager.RemoveFromRoleAsync(user, Role.NoKyc);

            // restore actual role
            if (!string.IsNullOrEmpty(user.TempRole))
            {
                await _userManager.AddToRoleAsync(user, user.TempRole!);

            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectKycAsync(string userId, string remarks)
        {
            var kyc = await _db.UserData.FirstOrDefaultAsync(u => u.UserDataId == userId);
            var user = await _db.ApplicationUser.FirstOrDefaultAsync(_db => _db.Id == userId);
            if (kyc == null || user == null) return false;

            kyc.Status = KycStatus.Rejected.ToString();
            kyc.AdminRemarks = remarks;

            user.KycStatus = false;
            user.KycApproveConfirmed = false;

            // leave them in NoKyc role so they can re-submit
            if (!await _userManager.IsInRoleAsync(user, Role.NoKyc))
                await _userManager.AddToRoleAsync(user, Role.NoKyc);

            await _db.SaveChangesAsync();
            return true;
        }


    }

}
