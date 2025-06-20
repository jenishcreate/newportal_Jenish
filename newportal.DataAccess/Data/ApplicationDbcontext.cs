using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using newportal.Models;



namespace newportal.DataAccess.Data
{
    public class ApplicationDbcontext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<UserData> UserData { get; set; }
        public DbSet<Wallet> wallet { get; set; }
        public DbSet<Useravailableservices> Useravailableservices { get; set; }
        public DbSet<UserCommission> UserCommissions { get; set; }
        public DbSet<Adharpanverification> Adharpanverifications { get; set; }
        public DbSet<RechargeTransaction> RechargeTransactions { get; set; }
        public DbSet<CreditCard_Bill_Transaction> CreditCardBillTransactions { get; set; }
        public DbSet<WalletTransaction> WalletTransaction { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var user = new IdentityUser(); // Create a user instance
            var hasher = new PasswordHasher<IdentityUser>();
            string hashedPassword = hasher.HashPassword(user, "Admin@123");
            Console.WriteLine(hashedPassword);

        }

    }
}
