using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace newportal.Models.ViewModel
{
    public class UserData_VM
    {
        [ValidateNever]
        public ApplicationUser? AppUser { get; set; }


        public UserData? UserData { get; set; }

        [Required]
        public IFormFile? AadhaarFront { get; set; }

        [Required]
        public IFormFile? AadhaarBack { get; set; }

        [Required]
        public IFormFile? PanImage { get; set; }

        [Required]
        public IFormFile? ProfilePicture { get; set; }

        [Required]
        public IFormFile? Blankcheque { get; set; }


        public IFormFile? UdhyamAdhar { get; set; }
    }



}

