using System.ComponentModel.DataAnnotations;

namespace newportal.Models.ViewModel.Auth
{
    public class Otp_VM
    {
        [Required]
        [Display(Name = "OTP")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits.")]
        public string InputOtp { get; set; }

        public Auth_VM auth { get; set; }

        [Display(Name = "Remember this device")]
        public bool RememberMachine { get; set; }

    }

}
