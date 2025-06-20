using newportal.Models;
using newportal.Utility;

namespace newportal.DataAccess.Repository.Interfaces
{
    public interface IUserCommissionRepository
    {
        Task<bool> CreateAsync(UserCommission obj);
        Task<UserCommission?> GetByIdAsync(string userId);
        Task<IEnumerable<UserCommission>> GetAllUserByRole(String Rolename);
     
        Task<bool> UpdateAsync(UserCommission obj);
        Task<bool> DeleteAsync(string userId);
    }
}
