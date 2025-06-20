using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using newportal.DataAccess.Data;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;

namespace newportal.DataAccess.Repository
{
    public class RechargeTransactionRepository : IRechargeTransactionRepository
    {
        private readonly ApplicationDbcontext _context;

        public RechargeTransactionRepository(ApplicationDbcontext context)
        {
            _context = context;
        }

        public async Task<bool> AddRechargeTransactionAsync(RechargeTransaction transaction)
        {
            await _context.RechargeTransactions.AddAsync(transaction);
            int res = await _context.SaveChangesAsync();
            if(res > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<RechargeTransaction>> GetAllTransactionsAsync()
        {
            return await _context.RechargeTransactions
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<RechargeTransaction>> GetSelectedFieldsByUserIdAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            DateTime startOfDay = startDate.Date;
            DateTime endOfDay = endDate.Date.AddDays(1).AddTicks(-1);

            return await _context.RechargeTransactions
                .Where(t => t.Initiated_UserId == userId &&
                            t.TransactionDate >= startOfDay &&
                            t.TransactionDate <= endOfDay)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new RechargeTransaction
                {
                    Recharge_transaction_id = t.Recharge_transaction_id,
                    Initiated_UserName = t.Initiated_UserName,
                    Customer_MobileNumber = t.Customer_MobileNumber,
                    MobileNumber_operator = t.MobileNumber_operator,
                    Last_updatedbalance = t.Last_updatedbalance,
                    Circle = t.Circle,
                    Description = t.Description,
                    Gst = t.Gst,
                    RechargeType = t.RechargeType,
                    RechargeAmount = t.RechargeAmount,
                    debited_Amount = t.debited_Amount,
                    Status = t.Status,
                    TransactionDate = t.TransactionDate
                })
                .ToListAsync();
        }


        public async Task<RechargeTransaction?> GetTransactionByIdAsync(string transactionId)
        {
            return await _context.RechargeTransactions
                .FirstOrDefaultAsync(t => t.Recharge_transaction_id == transactionId);
        }

        public async Task<IEnumerable<RechargeTransaction>> GetTransactionByParentIdAsync(string userId, DateTime startDate, DateTime endDate)
        {
            DateTime startOfDay = startDate.Date;
            DateTime endOfDay = endDate.Date.AddDays(1).AddTicks(-1);

            return await _context.RechargeTransactions
                .Where(t => t.Initiated_User_Parentid == userId ||t.Initiated_User_L2_Parentid == userId &&
                            t.TransactionDate >= startOfDay &&
                            t.TransactionDate <= endOfDay)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new RechargeTransaction
                {
                    Recharge_transaction_id = t.Recharge_transaction_id,
                    Initiated_UserId = t.Initiated_UserId,
                    Initiated_UserName = t.Initiated_UserName,
                    Initiated_companyname = t.Initiated_companyname,
                    Initiated_User_number = t.Initiated_User_number,
                    Customer_MobileNumber = t.Customer_MobileNumber,
                    MobileNumber_operator = t.MobileNumber_operator,
                    Circle = t.Circle,
                    Description = t.Description,
                    Gst = t.Gst,
                    RechargeType = t.RechargeType,
                    Last_updatedbalance = t.Last_updatedbalance,
                    RechargeAmount = t.RechargeAmount,
                    debited_Amount = t.debited_Amount,
                    Status = t.Status,
                    BBPS_Resopnce_Statuscode = t.BBPS_Resopnce_Statuscode,
                    BBPS_status = t.BBPS_status,
                    TransactionDate = t.TransactionDate
                })
                .ToListAsync();
        }

        public async Task UpdateTransactionAsync(RechargeTransaction transaction)
        {
            _context.RechargeTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(string transactionId)
        {
            var transaction = await _context.RechargeTransactions
                .FirstOrDefaultAsync(t => t.Recharge_transaction_id == transactionId);

            if (transaction != null)
            {
                _context.RechargeTransactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }
    }
}
