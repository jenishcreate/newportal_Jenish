using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V5.Pages.Account.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using newportal.DataAccess.Data;
using newportal.DataAccess.Repository.Interfaces;
using newportal.DataAccess.Repository.IRepository;

namespace newportal.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbcontext _db;
        public RoleManager<IdentityRole> _roleManager { get; }
        public IRoleRepository Role { get; private set; }
        public IUserRepository User { get; private set; }
        public IWalletRepository Wallet { get; private set; }
        public IKycRepository Kyc { get; private set; }
        public IUserAvailableServices userAvailableServices { get; private set; } 
        public IUserCommissionRepository userCommissionRepository { get; private set; }
        public IAdharpanverification adharpanverification { get; private set; }
        public IListRepository List { get; private set; }
        public IRechargeTransactionRepository Rechargetransaction { get; private set; }
        public IAuthRepository Auth { get; private set; }


        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly EmailSender _emailSender;
        private readonly PhoneSender _phoneSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;





        public UnitOfWork(ApplicationDbcontext db, UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            EmailSender emailSender, RoleManager<IdentityRole> roleManager,
            PhoneSender phoneSender,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment env, IConfiguration config)

        {

            _db = db ?? throw new ArgumentNullException(nameof(db));
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _env = env;
            _phoneSender = phoneSender;
            _httpContextAccessor = httpContextAccessor;
            Role = new RoleRepository(_db, roleManager);
            User = new UserRepository(_db, userManager, userStore, signInManager, logger, roleManager, emailSender,env);
            Wallet = new WalletRepository(_db);
            Kyc = new KycRepository(_db, roleManager, userManager);
            List = new ListRepository(_db,roleManager,userManager,config);
            Auth = new AuthRepository(_db, userManager, signInManager, emailSender, phoneSender, httpContextAccessor);
            adharpanverification = new AdharpanverificationRepository(_db);
            userAvailableServices = new UserAvailableServices(_db);
            userCommissionRepository = new UserCommissionRepository(_db);
            Rechargetransaction = new RechargeTransactionRepository(_db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
