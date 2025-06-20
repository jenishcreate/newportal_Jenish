using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newportal.Models
{

    public class UserData
    {
        [Key]
        public string UserNo { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("ApplicationUserId")]
        public string? UserDataId { get; set; }

        [Required]
        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

        [Required]  
        public string? LastName { get; set; }

        [Required]
        public string? DOB { get; set; }

        [Required]
        public string? Gender { get; set; }


        [Required]
        public string? Adhar_No { get; set; }


        [Required]
        public string? Pan_No { get; set; }


        [Required]
        public string? Address { get; set; }

        [Required]
        public string? City { get; set; }

        [Required]
        public string? State { get; set; }

        [Required]
        public string? Country { get; set; }

        [Required]
        public string? PinCode { get; set; }


        public string? GST_NO { get; set; }


        public string? Udhyam_Adhar_NO { get; set; }


        public string? ProfilePicture { get; set; }

        public string? Adhar_Front_Photo_Url { get; set; }


        public string? Adhar_Back_Photo_Url { get; set; }


        public string? Pan_Photo_Url { get; set; }


        public string? Blank_Check_Photo_Url { get; set; }

        public bool? KYC_Submitted { get; set; }
        public string Status { get; set; } = KycStatus.Pending.ToString();

        public string? Companyname { get; set; }
        public string? AdminRemarks { get; set; }

    }
    public enum KycStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
