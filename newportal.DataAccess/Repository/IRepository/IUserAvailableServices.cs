using newportal.Models;

namespace newportal.DataAccess.Repository.IRepository
{
    public interface IUserAvailableServices
    {
        Task<bool> CreateAsync(Useravailableservices obj);
        Task<Useravailableservices> GetUserServiceByIdAsync(string userId);
        Task<IEnumerable<Useravailableservices>> GetAllUsersListAsync();
        Task<bool> UpdateAsync(Useravailableservices obj);

        Task<List<Useravailableservices>> GetRetailerUserAvailableServicesAsync();
        
     }
}
