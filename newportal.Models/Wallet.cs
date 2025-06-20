using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newportal.Models
{
    public class Wallet
    {
        [Key]
        public string WalletId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("ApplicationUserId")]
        public string? UserId { get; set; }
        //[Required]
        //public string? WalletName { get; set; }
        [Required]
        public double? WalletBalance { get; set; }
        [Required]
        public bool? WalletStatus { get; set; } = true;

        public double? WalletBlockAmount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;



    }
}
