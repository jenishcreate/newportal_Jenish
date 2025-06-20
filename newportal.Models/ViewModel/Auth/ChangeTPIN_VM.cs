using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newportal.Models.ViewModel.Auth
{
    public class ChangeTPIN_VM
    {
        [Required(ErrorMessage = "OTP is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        [Display(Name = "OTP")]
        public string OTP { get; set; }

        [Required(ErrorMessage = "New TPIN is required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "TPIN must be 4 digits")]
        [DataType(DataType.Password)]
        [Display(Name = "New TPIN")]
        public string NewTPIN { get; set; }

        [Required(ErrorMessage = "Confirm TPIN is required")]
        [DataType(DataType.Password)]
        [Compare("NewTPIN", ErrorMessage = "TPIN and Confirm TPIN do not match")]
        [Display(Name = "Confirm TPIN")]
        public string ConfirmTPIN { get; set; }
    }
}
