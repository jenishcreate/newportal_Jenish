using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newportal.Models
{
    public class Useravailableservices
    {
        /// <summary>
        /// We use the ApplicationUser's Id as our PK & FK.
        /// </summary>
        [Key]
        public string UseravailableservicesId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("ApplicationUserId")]
        public string ApplicationUserId { get; set; }
        public string? UserName { get; set; }
        public bool DmtService { get; set; } = true;
        public bool CreditCardBill { get; set; } = true;
        public bool CreditCardBbps { get; set; } = true;
        public bool RechargeService { get; set; } = true;
        public bool DthService { get; set; } = true;
        public bool ElectricityBillService { get; set; } = true;
        public bool WaterBillService { get; set; } = true;
        public bool InsuranceService { get; set; } = true;
        public bool MoveToBankService { get; set; } = true;
        public bool IndoNepalService { get; set; } = true;
        public bool TravelBookingService { get; set; } = true;
        public bool PosService { get; set; } = true;
        public bool CmsService { get; set; } = true;
        public bool LoansAndCreditCardService { get; set; } = true;
        public bool AepsService { get; set; } = true;
    }
}
