
using Microsoft.EntityFrameworkCore;
using newportal.DataAccess.Data;
using newportal.DataAccess.Repository.Interfaces;
using newportal.Models;
using newportal.Utility;

namespace newportal.DataAccess.Repository
{
    public class UserCommissionRepository : IUserCommissionRepository
    {
        private readonly ApplicationDbcontext _context;

        public UserCommissionRepository(ApplicationDbcontext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(UserCommission obj)
        {
            if (await _context.UserCommissions.FindAsync(obj.ApplicationUserId) != null)
                return false;

            _context.UserCommissions.Add(obj);
            int num = await _context.SaveChangesAsync();
            if (num <= 0)
                return false;
            return true;
        }

        public async Task<UserCommission?> GetByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
            }
            return await _context.UserCommissions
               
                .FirstOrDefaultAsync(u => u.ApplicationUserId == userId);
        }

        public async Task<IEnumerable<UserCommission>> GetAllUserByRole(string Rolename)
        {


            var RoleId = await _context.Roles
                .Where(r => r.Name == Rolename)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var UserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == RoleId)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var commissions = await _context.UserCommissions
                .Where(s => UserIds.Contains(s.ApplicationUserId))
                .ToListAsync();

            // Populate the UserName from ApplicationUser
            var userNames = await _context.ApplicationUser
                .Where(u => UserIds.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            // Create a dictionary for fast lookup
            var userMap = userNames.ToDictionary(u => u.Id, u => u.UserName);

            foreach (var commission in commissions)
            {
                if (userMap.TryGetValue(commission.ApplicationUserId, out var userName))
                {
                    commission.Username = userName;
                }
            }

            return commissions;
        }

       


        public async Task<bool> UpdateAsync(UserCommission obj)
        {
            var existing = await _context.UserCommissions.FirstOrDefaultAsync(u => u.ApplicationUserId == obj.ApplicationUserId);
            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(obj);
            int num = await _context.SaveChangesAsync();
            if (num <= 0)
                return false;
            return true;
        }

        public async Task<bool> DeleteAsync(string userId)
        {
            var commission = await _context.UserCommissions.FindAsync(userId);
            if (commission == null)
                return false;

            _context.UserCommissions.Remove(commission);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
