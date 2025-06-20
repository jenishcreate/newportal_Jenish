using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using newportal.DataAccess.Data;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using System.Linq.Expressions;


namespace newportal.DataAccess.Repository
{
    public class RoleRepository : Repository<IdentityRole>, IRoleRepository
    {
        private readonly ApplicationDbcontext _db;
        private readonly RoleManager<IdentityRole> _roleManager;



        public RoleRepository(ApplicationDbcontext db, RoleManager<IdentityRole> roleManager) : base(db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public void Add(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public bool Check(string role)
        {
            return !_db.Roles.Any(x => x.Name == role);
        }

        public async Task CreateAsync(string role)
        {

            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        public ApplicationUser Get(Expression<Func<ApplicationUser, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ApplicationUser> GetAll(Expression<Func<ApplicationUser, bool>>? filter = null, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public List<string?> GetAllRoles()
        {
            return _roleManager.Roles.Select(x => x.Name).ToList();
        }

        public IEnumerable<SelectListItem> GetAllRolesitem()
        {

            return _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
            {
                Text = i,
                Value = i
            });

        }

        public void Remove(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<ApplicationUser> entity)
        {
            throw new NotImplementedException();
        }
    }
}
