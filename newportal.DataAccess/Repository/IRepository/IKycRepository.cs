using newportal.Models;
using newportal.Models.ViewModel;

namespace newportal.DataAccess.Repository.IRepository
{
    public interface IKycRepository : IRepository<UserData>
    {
        Task<bool> SubmitKycAsync(UserData_VM model);
        Task<UserData> GetKycDetailsByUserId(string userId);
        Task<List<UserData>> GellAllKycoList();
        Task<bool> UpdateKycAsync(UserData userData);
        Task<bool> ApproveKycAsync(string userId);
        Task<bool> RejectKycAsync(string userId, string remarks);




    }
}
