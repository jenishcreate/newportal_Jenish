using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using newportal.Models;

namespace newportal.DataAccess.Repository.IRepository
{
    public interface IRechargeTransactionRepository
    {
        Task<bool> AddRechargeTransactionAsync(RechargeTransaction transaction);
        Task<IEnumerable<RechargeTransaction>> GetAllTransactionsAsync();
        Task<IEnumerable<RechargeTransaction>> GetSelectedFieldsByUserIdAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
        Task<RechargeTransaction?> GetTransactionByIdAsync(string transactionId);
        Task<IEnumerable<RechargeTransaction>> GetTransactionByParentIdAsync(string userId, DateTime startDate, DateTime endDate); // Optional if you have parent-child user relation
        Task UpdateTransactionAsync(RechargeTransaction transaction);
        Task DeleteTransactionAsync(string transactionId);
    }
}
