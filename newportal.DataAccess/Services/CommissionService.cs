using Microsoft.EntityFrameworkCore;
using newportal.DataAccess.Data;
using newportal.Models;

namespace newportal.DataAccess.Services
{
    public class CommissionService
    {
        private readonly ApplicationDbcontext _db;

        public CommissionService(ApplicationDbcontext db)
        {
            _db = db;

        }

        /// <summary>
        /// Returns (userId, ratePct, commissionAmount) for each link in the chain,
        /// starting with the transacting user and going up to the top.
        /// </summary>

        public async Task<List<(string userId, decimal spreadPercent, decimal commissionAmount)>>
     CalculateChainCommissionsAsync(string userId, decimal transactionAmount)
        {
            var result = new List<(string userId, decimal spreadPercent, decimal commissionAmount)>();

            var currentUser = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId);

            if (currentUser == null)
                throw new Exception("User not found");

            var chain = new List<ApplicationUser>();

            while (currentUser != null)
            {
                chain.Add(currentUser);

                if (string.IsNullOrEmpty(currentUser.ParentUserId))
                    break;

                currentUser = await _db.ApplicationUser
                    .FirstOrDefaultAsync(u => u.Id == currentUser.ParentUserId);
            }

            for (int i = 0; i < chain.Count; i++)
            {
                var me = chain[i];
                var meCommission = await _db.UserCommissions.FindAsync(me.Id);
                if (meCommission == null)
                    throw new Exception($"Missing commission entry for user {me.Id}");

                decimal myRate = meCommission.PayoutRate;

                decimal parentRate = 0;
                if (i + 1 < chain.Count)
                {
                    var parent = chain[i + 1];
                    var parentCommission = await _db.UserCommissions.FindAsync(parent.Id);
                    if (parentCommission != null)
                    {
                        parentRate = parentCommission.PayoutRate;
                    }
                }

                decimal spread = myRate - parentRate;
                decimal commissionAmount = transactionAmount * (spread / 100m);

                result.Add((me.Id, spread, commissionAmount));
            }

            return result;
        }
    }
}
