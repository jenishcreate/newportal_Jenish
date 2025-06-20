using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace newportal.Models
{
    public class ApplicationUser : IdentityUser
    {


        private static string GenerateFundTransferId(int length = 7)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return "FTID" + new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray()) + "@mobipay";
        }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public string? Firm_Name { get; set; }

        [Required]
        public string TPIN { get; set; }
        public string? TempRole { get; set; }
        public ApplicationUser()
        {
            FundTransferId = GenerateFundTransferId();
        }
        public string FundTransferId { get; set; }



        public string? ParentUserId { get; set; }

        public string? L2_ParentUserId { get; set; }

        public bool IsActive { get; set; } = true;

        [ProtectedPersonalData]
        [Required]
        public virtual string? WhatsAppPhoneNumber { get; set; }

        public bool KycStatus { get; set; } = false;

        public bool KycApproveConfirmed { get; set; } = false;

        /// <summary>
        /// Navigation to the row that holds this user's service‑flags.
        /// </summary>
        [ValidateNever]
        /// <summary>
        /// Navigation to the row that holds this user's service‑flags.
        /// </summary>
        public Useravailableservices? Services { get; set; }
        /// <summary>
        /// Commission rates per service for this user.
        /// </summary>
        [ValidateNever]
        public UserCommission? Commission { get; set; }
        [ValidateNever]
        public Adharpanverification? Verification { get; set; }
    }
}
