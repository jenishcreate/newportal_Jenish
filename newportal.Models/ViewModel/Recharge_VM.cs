using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newportal.Models.ViewModel
{
    public class Recharge_VM
    {
        public string? Initiated_UserId { get; set; }
        public string? Initiated_UserName { get; set; }
        public string? Customer_MobileNumber { get; set; }
        public string? Circle { get; set; }
        public string? MobileNumber_operator { get; set; }
        public string? RechargeType { get; set; }
        public decimal RechargeAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Tpin { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
