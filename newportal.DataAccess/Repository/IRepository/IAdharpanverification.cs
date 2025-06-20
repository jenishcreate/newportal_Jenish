using newportal.Models;

namespace newportal.DataAccess.Repository.IRepository
{
    public interface IAdharpanverification
    {
        Task<bool> CreateAsync(Adharpanverification obj);
        Task<Adharpanverification?> GetByIdAsync(string userId);
        Task<IEnumerable<Adharpanverification>> GetAllAsync();
        Task<bool> UpdateAsync(Adharpanverification obj);
        Task<bool> DeleteAsync(string userId);
    }
}
