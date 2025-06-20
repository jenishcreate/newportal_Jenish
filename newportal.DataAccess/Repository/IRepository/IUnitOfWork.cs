using newportal.DataAccess.Repository.Interfaces;

namespace newportal.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IRoleRepository Role { get; }
        IUserRepository User { get; }
        IWalletRepository Wallet { get; }
        IKycRepository Kyc { get; }
        IUserAvailableServices userAvailableServices { get; }
        IUserCommissionRepository userCommissionRepository { get;}
        IAdharpanverification adharpanverification { get; }
        IListRepository List { get; }
        IAuthRepository Auth { get; }
        IRechargeTransactionRepository Rechargetransaction { get; }
        Task SaveAsync();
    }
}
