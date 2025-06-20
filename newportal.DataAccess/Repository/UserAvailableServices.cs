
using Microsoft.EntityFrameworkCore;
using newportal.DataAccess.Data;
using newportal.DataAccess.Repository.IRepository;
using newportal.Models;
using newportal.Utility;

namespace newportal.DataAccess.Repository
{
    public class UserAvailableServices : IUserAvailableServices
    {
        private readonly ApplicationDbcontext _context;

        public UserAvailableServices(ApplicationDbcontext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(Useravailableservices obj)
        {
            if (await _context.Useravailableservices.FindAsync(obj.ApplicationUserId) != null)
                return false; // Avoid duplicate PK insertion

            _context.Useravailableservices.Add(obj);
            int num = await _context.SaveChangesAsync();
            if (num <= 0)
                return false;
            return true;
        }

        public async Task<List<Useravailableservices>> GetRetailerUserAvailableServicesAsync()
        {
            var retailerRoleId = await _context.Roles
                .Where(r => r.Name == Role.Retailer)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var retailerUserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == retailerRoleId)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var services = await _context.Useravailableservices
                .Where(s => retailerUserIds.Contains(s.ApplicationUserId))
                .ToListAsync();

            // Populate the UserName from ApplicationUser
            var userNames = await _context.ApplicationUser
                .Where(u => retailerUserIds.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            // Create a dictionary for fast lookup
            var userMap = userNames.ToDictionary(u => u.Id, u => u.UserName);

            foreach (var service in services)
            {
                if (userMap.TryGetValue(service.ApplicationUserId, out var userName))
                {
                    service.UserName = userName;
                }
            }

            return services;
        }




        public async Task<Useravailableservices?> GetUserServiceByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
            }
            return await _context.Useravailableservices
            
                .FirstOrDefaultAsync(x => x.ApplicationUserId == userId);
        }

        public async Task<IEnumerable<Useravailableservices>> GetAllUsersListAsync()
        {
            IEnumerable<Useravailableservices> servicelist = await _context.Useravailableservices
                .ToListAsync();
            return servicelist;

        }

        public async Task<bool> UpdateAsync(Useravailableservices obj)
        {
            var existing = await _context.Useravailableservices.FirstOrDefaultAsync(x => x.ApplicationUserId == obj.ApplicationUserId);
            if (existing == null)
                return false;

            // Don't overwrite UserName — just update the services
            existing.DmtService = obj.DmtService;
            existing.CreditCardBill = obj.CreditCardBill;
            existing.CreditCardBbps = obj.CreditCardBbps;
            existing.RechargeService = obj.RechargeService;
            existing.DthService = obj.DthService;
            existing.ElectricityBillService = obj.ElectricityBillService;
            existing.WaterBillService = obj.WaterBillService;
            existing.InsuranceService = obj.InsuranceService;
            existing.MoveToBankService = obj.MoveToBankService;
            existing.IndoNepalService = obj.IndoNepalService;
            existing.TravelBookingService = obj.TravelBookingService;
            existing.PosService = obj.PosService;
            existing.CmsService = obj.CmsService;
            existing.LoansAndCreditCardService = obj.LoansAndCreditCardService;
            existing.AepsService = obj.AepsService;

            int num = await _context.SaveChangesAsync();
            return num > 0;
        }


    }
}
