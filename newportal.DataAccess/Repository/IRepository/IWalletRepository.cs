using newportal.Models;
using static newportal.DataAccess.Repository.WalletRepository;

namespace newportal.DataAccess.Repository.IRepository
{
    public interface IWalletRepository
    {
        Task<bool> CreateWalletAsync(string id);

        Task<WalletOperationResult> CreditWalletbyadminAsync(string toUserId, double amount, string initiatedBy, string? remarks);

        Task<WalletOperationResult> DebitWalletAsync(string fromUserId, double amount, string initiatedBy, string remarks);

        Task<WalletOperationResult> TransferWalletAsync(string fromUserId, string toUserId, double amount, string initiatedBy, string remarks);

        Task<WalletOperationResult> RecordServiceUsageAsync(string userId, double amount, string linkedTransactionId, string remarks, string transtypr);

        Task<double> GetUserWalletBalanceAsync(string userId);

        Task<List<WalletTransaction>> GetWalletHistoryAsync(string userId , DateTime startDate, DateTime endDate);

        Task<List<WalletTransaction>> GetWalletTransactionsByTypeAsync(string userId, string type);

        Task<double> GetTotalSystemBalanceAsync();
        Task<Wallet> GetUserWalletDataAsync(string userId);
        Task<WalletOperationResult> ToggleWalletStatusAsync(string userId);
        Task Update(Wallet wallet);
        Task<bool> Checkuserbalanceforservice(string userId, double amount);
        Task<List<WalletTransaction>> GetwalletTransactionAdmin(DateTime startDate, DateTime endDate);
    }
}
