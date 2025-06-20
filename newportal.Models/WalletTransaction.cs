using System.ComponentModel.DataAnnotations;

namespace newportal.Models
{
    public class WalletTransaction
    {
        [Key]
        public string TransactionId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string FromUserId { get; set; }


        public string ToUserId { get; set; }

        public string? LinkedServiceTransactionId { get; set; } // Optional

        [Required]
        public double Amount { get; set; }
        public double Description { get; set; }


        public string TransactionType { get; set; } = "Transfer"; // Credit, Debit, Service, PeerTransfer

        public string InitiatedByUserId { get; set; } 

        public string Remarks { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}
