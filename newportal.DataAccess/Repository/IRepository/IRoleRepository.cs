using Microsoft.AspNetCore.Mvc.Rendering;
using newportal.Models;


namespace newportal.DataAccess.Repository.IRepository
{
    public interface IRoleRepository : IRepository<ApplicationUser>
    {
        List<string?> GetAllRoles();
        IEnumerable<SelectListItem> GetAllRolesitem();
        Task CreateAsync(string role);
        bool Check(string role);
    }
}
