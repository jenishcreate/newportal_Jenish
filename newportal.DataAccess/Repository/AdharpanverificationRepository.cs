using Microsoft.EntityFrameworkCore;
using newportal.DataAccess.Data;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;

namespace newportal.DataAccess.Repository
{
    public class AdharpanverificationRepository : IAdharpanverification
    {
        private readonly ApplicationDbcontext _context;

        public AdharpanverificationRepository(ApplicationDbcontext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(Adharpanverification obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // Fix: Replace AllAsync with AddAsync to add the object to the DbSet
            await _context.Adharpanverifications.AddAsync(obj);
            await _context.SaveChangesAsync(); // Ensure SaveChangesAsync is awaited
            return true; // Assuming the operation is successful
        }



        public async Task<IEnumerable<Adharpanverification>> GetAllAsync()
        {


            IEnumerable<Adharpanverification> verificationlist = await _context.Adharpanverifications
                .ToListAsync();
            return verificationlist;
        }

        public async Task<Adharpanverification?> GetByIdAsync(string userId)
        {
            Adharpanverification? adharpanverification = await _context.Adharpanverifications
                .FirstOrDefaultAsync(a => a.ApplicationUserId == userId);

            if (adharpanverification == null)
            {
                NotFiniteNumberException ex = new NotFiniteNumberException($"No Adharpanverification found for userId: {userId}");
            }
            return adharpanverification;
        }

        public async Task<bool> UpdateAsync(Adharpanverification obj)
        {
            await _context.SaveChangesAsync(); // Ensure SaveChangesAsync is awaited
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            var existing = await _context.Adharpanverifications.FindAsync(obj.ApplicationUserId);
            if (existing == null)
                return false;
            _context.Entry(existing).CurrentValues.SetValues(obj);
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<bool> DeleteAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
