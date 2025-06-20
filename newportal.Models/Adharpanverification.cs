using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newportal.Models
{
    public class Adharpanverification
    {
        [Key]
        public string AdharpanverificationId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("ApplicationUserId")]
        public string ApplicationUserId { get; set; }

        public bool? Adhar_verifide { get; set; } = false;
        public bool? Email_verifide { get; set; } = false;
        public bool? PhoneNo_verifide { get; set; } = false;
        public bool? WhatsappNo_verifide { get; set; } = false;
        public bool? Pan_verifide { get; set; } = false;


    }
}
