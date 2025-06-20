using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newportal.Models.ViewModel
{
    public class Creditcard_Bbps_VM
    {
        public string? Initiated_UserId { get; set; }
        public string? Initiated_User_IP { get; set; }
        public string? Initiated_Username { get; set; }
        public string? Initiated_MobileNumber { get; set; }
        public string? Initiated_companyname { get; set; }
        public double? User_Latitude { get; set; }
        public double? User_Longitude { get; set; }
        public string? Initiated_User_Parent_Number { get; set; }
        public string? Initiated_User_Parentid { get; set; }
        public string? Initiated_User_ParentCompany { get; set; }
        public string? Initiated_User_L2_Parent_Number { get; set; }
        public string? Initiated_User_L2_Parentid { get; set; }
        public string? Initiated_User_L2_ParentCompany { get; set; }
        [Required]
        public string? Customer_MobileNumber { get; set; }
        
        public string? Card_Owner_Name { get; set; }

        [Required]
        public string? Customer_Card_number { get; set; }

        [Required]
        public string? CreditCard_Bank { get; set; }
        public string? CreditCard_Operator { get; set; }
        public string? CreditCard_Catodary { get; set; }

        public decimal Bill_Amount_Pay { get; set; }

        public decimal Bill_Amount_Fetch { get; set; }
        public DateTime Due_date { get; set; }
        [Required]
        public string? Tpin { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
