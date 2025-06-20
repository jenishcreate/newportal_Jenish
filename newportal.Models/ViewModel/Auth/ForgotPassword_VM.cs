using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newportal.Models.ViewModel.Auth
{
    public class ForgotPassword_VM
    
    {
        [Required]
        
        public string UserId { get; set; }

        
    }
}
