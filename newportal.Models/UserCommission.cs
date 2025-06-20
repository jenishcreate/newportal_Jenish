using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newportal.Models
{
    public class UserCommission
    {
        /// <summary>
        /// Holds the commission‐percent (in decimal form) that each user
        /// (agent/distributor/admin) earns on each service transaction.
        /// PK & FK → ApplicationUser.Id
        /// </summary>

        [Key]
        public string UserCommissionId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("ApplicationUserId")]
        public string ApplicationUserId { get; set; }

        
        public string Username { get; set; }


        [Column(TypeName = "decimal(5,3)")]
        public decimal Dmt { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal Recharge { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal Dth { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal ElectricityBill { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal WaterBill { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal Insurance { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal CcBill { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal CcBillBbps { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal PayoutRate { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal MoveToBank { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal IndoNepal { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal TravelBooking { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal Pos { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal Cms { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal LoansAndCreditCard { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal Aeps { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal PG1 { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal PG2 { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal PG3 { get; set; } = 0m;

        [Column(TypeName = "decimal(5,3)")]
        public decimal PG4 { get; set; } = 0m;

       
      
    }
}
