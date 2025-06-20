using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newportal.Models
{
    public class RechargeTransaction
    {
        [Key]
        public string? Recharge_transaction_id { get; set; } = Guid.NewGuid().ToString();
        public string? Initiated_UserId { get; set; }
        public string? Initiated_User_IP { get; set; }
        public string? Initiated_User_number { get; set; }
        public string? Initiated_companyname { get; set; }
        public double? User_Latitude { get; set; }
        public double? User_Longitude { get; set; }
        public string? Initiated_UserName { get; set; }
        public string? Initiated_User_Parent_Number { get; set; }
        public string? Initiated_User_Parentid { get; set; }
        public string? Initiated_User_ParentCompany { get; set; }
        public string? Initiated_User_L2_Parent_Number { get; set; }
        public string? Initiated_User_L2_Parentid { get; set; }
        public string? Initiated_User_L2_ParentCompany { get; set; }
        public string? Customer_MobileNumber { get; set; }
        public string? MobileNumber_operator { get; set; }
        public decimal Last_updatedbalance  { get; set; }
        public string? Circle { get; set; }
        public string? RechargeType { get; set; }
        public decimal Bill_Amount_Fetch { get; set; }
        public decimal RechargeAmount { get; set; }
        public decimal debited_Amount { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public decimal Gst { get; set; }
        public string? BBPS_Resopnce_Statuscode { get; set; }
        public string? BBPS_TransactionId { get; set; }
        public string? BBPS_Operator_Id { get; set; }
        public string? BBPS_status { get; set; } 
        public DateTime TransactionDate { get; set; } = DateTime.Now;

    }
}
