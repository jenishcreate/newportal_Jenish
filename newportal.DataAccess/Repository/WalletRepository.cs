using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using newportal.DataAccess.Data;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;

namespace newportal.DataAccess.Repository
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbcontext _context;
        public WalletRepository(ApplicationDbcontext db)
        {
            _context = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<bool> CreateWalletAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(id));
            }
            _context.wallet.AddAsync(new Wallet
            {
                UserId = id,
                WalletBalance = 0,
                WalletStatus = true,
                WalletBlockAmount = 0
            });
            int num = await _context.SaveChangesAsync();
            if (num <= 0)
            {
                return false; // If no rows were affected, return false
            }
            return true; // If the operation was successful, return true

        }

        public async Task<WalletOperationResult> CreditWalletbyadminAsync(string toUserId, double amount, string initiatedBy, string? remarks)
        {
            var wallet = await _context.wallet.FirstOrDefaultAsync(w => w.UserId == toUserId && w.WalletStatus == true);
            if (wallet == null)
                return new WalletOperationResult { Success = false, Message = "Wallet not found or is blocked." };

            wallet.WalletBalance += amount;

            var transaction = new WalletTransaction
            {
                FromUserId = "SYSTEM",
                ToUserId = toUserId,
                Amount = amount,
                TransactionType = "Credit",
                InitiatedByUserId = initiatedBy,
                Remarks = remarks ?? "Admin credit operation"
            };

            _context.WalletTransaction.Add(transaction);
            await _context.SaveChangesAsync();

            return new WalletOperationResult { Success = true, Message = "Amount credited successfully.", TransactionId = transaction.TransactionId };
        }

        public async Task<WalletOperationResult> DebitWalletAsync(string fromUserId, double amount, string initiatedBy, string remarks)
        {
            var wallet = await _context.wallet.FirstOrDefaultAsync(w => w.UserId == fromUserId && w.WalletStatus == true);
            if (wallet == null)
                return new WalletOperationResult { Success = false, Message = "Wallet not found or is blocked." };

            double availableBalance = wallet.WalletBalance.GetValueOrDefault() - wallet.WalletBlockAmount.GetValueOrDefault();
            if (availableBalance < amount)
                return new WalletOperationResult { Success = false, Message = "Insufficient available balance (after block)." };

            wallet.WalletBalance -= amount;

            var transaction = new WalletTransaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                FromUserId = fromUserId,
                ToUserId = "SYSTEM",
                Amount = amount,
                TransactionType = "Debit",
                InitiatedByUserId = initiatedBy,
                Remarks = string.IsNullOrWhiteSpace(remarks) ? "System debit operation" : remarks,
                Date = DateTime.Now
            };

            try
            {
                _context.WalletTransaction.Add(transaction);
                await _context.SaveChangesAsync();

                return new WalletOperationResult { Success = true, Message = "Wallet debited successfully.", TransactionId = transaction.TransactionId };
            }
            catch (Exception ex)
            {
                // Log to console or use ILogger
                Console.WriteLine("EF Core Save Failed: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner: " + ex.InnerException.Message);

                return new WalletOperationResult { Success = false, Message = "Internal error: " + ex.Message };
            }
        }

        public async Task<WalletOperationResult> TransferWalletAsync(string fromUserId, string toUserId, double amount, string initiatedBy, string remarks)
        {
            var fromWallet = await _context.wallet.FirstOrDefaultAsync(w => w.UserId == fromUserId && w.WalletStatus == true);
            var toWallet = await _context.wallet.FirstOrDefaultAsync(w => w.UserId == toUserId && w.WalletStatus == true);

            if (fromWallet == null)
                return new WalletOperationResult { Success = false, Message = "Sender wallet not found or is blocked." };

            if (toWallet == null)
                return new WalletOperationResult { Success = false, Message = "Receiver wallet not found or is blocked." };

            double availableBalance = fromWallet.WalletBalance.GetValueOrDefault() - fromWallet.WalletBlockAmount.GetValueOrDefault();
            if (availableBalance < amount)
                return new WalletOperationResult { Success = false, Message = "Insufficient available balance (after block)." };

            fromWallet.WalletBalance -= amount;
            toWallet.WalletBalance += amount;

            string baseTransactionId = Guid.NewGuid().ToString();

            var senderTransaction = new WalletTransaction
            {
                TransactionId = baseTransactionId + "-S",
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Amount = amount,
                TransactionType = "PeerTransfer",
                InitiatedByUserId = initiatedBy,
                Remarks = "[Sent] " + remarks
            };

            var receiverTransaction = new WalletTransaction
            {
                TransactionId = baseTransactionId + "-R",
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Amount = amount,
                TransactionType = "PeerTransfer",
                InitiatedByUserId = initiatedBy,
                Remarks = "[Received] " + remarks
            };

            _context.WalletTransaction.AddRange(senderTransaction, receiverTransaction);
            await _context.SaveChangesAsync();

            return new WalletOperationResult { Success = true, Message = "Funds transferred successfully.", TransactionId = senderTransaction.TransactionId };
        }

        public async Task<WalletOperationResult> RecordServiceUsageAsync(string userId, double amount, string linkedTransactionId, string remarks , string transtypr)
        {
            var wallet = await _context.wallet.FirstOrDefaultAsync(w => w.UserId == userId && w.WalletStatus == true);
            if (wallet == null)
                return new WalletOperationResult { Success = false, Message = "Wallet not found or is blocked." };

            double availableBalance = wallet.WalletBalance.GetValueOrDefault() - wallet.WalletBlockAmount.GetValueOrDefault();
            if (availableBalance < amount)
                return new WalletOperationResult { Success = false, Message = "Insufficient balance for service." };

            wallet.WalletBalance -= amount;

            var transaction = new WalletTransaction
            {
                FromUserId = userId,
                ToUserId = "null",
                LinkedServiceTransactionId = linkedTransactionId,
                Amount = amount,
                TransactionType = transtypr,
                InitiatedByUserId = userId,
                Remarks = remarks
            };

            _context.WalletTransaction.Add(transaction);
            await _context.SaveChangesAsync();

            return new WalletOperationResult { Success = true, Message = "Service charge recorded.", TransactionId = transaction.TransactionId };
        }

        public async Task<bool> Checkuserbalanceforservice(string userId, double amount)
        {
            var wallet = await _context.wallet.FirstOrDefaultAsync(w => w.UserId == userId && w.WalletStatus == true);
            if (wallet == null)
                return false;

            double availableBalance = wallet.WalletBalance.GetValueOrDefault() - wallet.WalletBlockAmount.GetValueOrDefault();
            if (availableBalance < amount)
            {
                return false;
            }
                return true;
        }

        public async Task<double> GetUserWalletBalanceAsync(string userId)
        {
            var wallet = await _context.wallet.FirstOrDefaultAsync(w => w.UserId == userId);
            return wallet?.WalletBalance ?? 0;
        }
        public async Task<Wallet> GetUserWalletDataAsync(string userId)
        {
            var wallet = await _context.wallet.FirstOrDefaultAsync(w => w.UserId == userId);
            return wallet;
        }

        public async Task<List<WalletTransaction>> GetWalletHistoryAsync(string userId, DateTime startDate, DateTime endDate)
        {

            DateTime startDateNormalized = startDate.Date;
            DateTime endDateExclusive = endDate.Date.AddDays(1);

            var transactions = await _context.WalletTransaction
                 .Where(t => t.FromUserId == userId || t.ToUserId == userId)
                .Where(t => t.Date >= startDateNormalized && t.Date < endDateExclusive)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            var users = await _context.ApplicationUser
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            var userDict = users.ToDictionary(u => u.Id, u => u.UserName);

            var result = transactions.Select(t => new WalletTransaction
            {
                TransactionId = t.TransactionId,
                FromUserId = t.FromUserId == "SYSTEM" ? "SYSTEM" : userDict.GetValueOrDefault(t.FromUserId, "Unknown"),
                ToUserId = t.ToUserId == "SYSTEM" ? "SYSTEM" : userDict.GetValueOrDefault(t.ToUserId, "NULL"),
                InitiatedByUserId = t.InitiatedByUserId == "SYSTEM" ? "SYSTEM" : userDict.GetValueOrDefault(t.InitiatedByUserId, "Unknown"),
                Amount = t.Amount,
                Description = t.Description,
                TransactionType = t.TransactionType,
                LinkedServiceTransactionId=t.LinkedServiceTransactionId,
                Remarks = t.Remarks,
                Date = t.Date
            })
            .OrderByDescending(x => x.Date)
            .ToList();

            return result;
        }
        public async Task Update(Wallet wallet)
        {
            _context.wallet.Update(wallet);
          
        }


        public async Task<List<WalletTransaction>> GetwalletTransactionAdmin(DateTime startDate, DateTime endDate)
        {
            // Normalize startDate to 00:00:00 and endDate to the next day at 00:00:00 (exclusive)
            DateTime startDateNormalized = startDate.Date;
            DateTime endDateExclusive = endDate.Date.AddDays(1);

            var transactions = await _context.WalletTransaction
                .Where(t => t.Date >= startDateNormalized && t.Date < endDateExclusive)
                .ToListAsync();

            var users = await _context.ApplicationUser
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            var userDict = users.ToDictionary(u => u.Id, u => u.UserName);

            var result = transactions.Select(t => new WalletTransaction
            {
                TransactionId = t.TransactionId,
                FromUserId = t.FromUserId == "SYSTEM" ? "SYSTEM" : userDict.GetValueOrDefault(t.FromUserId, "Unknown"),
                ToUserId = t.ToUserId == "SYSTEM" ? "SYSTEM" : userDict.GetValueOrDefault(t.ToUserId, "Unknown"),
                InitiatedByUserId = t.InitiatedByUserId == "SYSTEM" ? "SYSTEM" : userDict.GetValueOrDefault(t.InitiatedByUserId, "Unknown"),
                Amount = t.Amount,
                Description = t.Description,
                LinkedServiceTransactionId = t.LinkedServiceTransactionId,
                TransactionType = t.TransactionType,
                Remarks = t.Remarks,
                Date = t.Date
            })
            .OrderByDescending(x => x.Date)
            .ToList();

            return result;
        }




        public async Task<List<WalletTransaction>> GetWalletTransactionsByTypeAsync(string userId, string type)
        {
            return await _context.WalletTransaction
                .Where(t => (t.FromUserId == userId || t.ToUserId == userId) && t.TransactionType.ToLower() == type.ToLower())
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<double> GetTotalSystemBalanceAsync()
        {
            return await _context.wallet
                .Where(w => w.WalletStatus == true)
                .SumAsync(w => w.WalletBalance ?? 0);
        }

        public async Task<WalletOperationResult> ToggleWalletStatusAsync(string userId)
        {
            var wallet = await _context.wallet.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
                return new WalletOperationResult { Success = false, Message = "Wallet not found." };

            wallet.WalletStatus = !wallet.WalletStatus;
            await _context.SaveChangesAsync();

            string statusText = (bool)wallet.WalletStatus ? "unblocked" : "blocked";
            return new WalletOperationResult { Success = true, Message = $"Wallet has been {statusText}." };
        }


        public async Task<WalletOperationResult> SetBlockAmountAsync(string userId, double blockAmount)
        {
            var wallet = await _context.wallet.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
                return new WalletOperationResult { Success = false, Message = "Wallet not found." };

            wallet.WalletBlockAmount = blockAmount;
            await _context.SaveChangesAsync();

            return new WalletOperationResult { Success = true, Message = $"Block amount set to ₹{blockAmount}" };
        }

      

        public class WalletOperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public string? TransactionId { get; set; } // Optional
        }


    }
}
