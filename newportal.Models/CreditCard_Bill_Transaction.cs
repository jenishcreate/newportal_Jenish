using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newportal.Models
{
    public class CreditCard_Bill_Transaction
    {
        [Key]
        public string CreditcardBillTransactionid { get; set; } = Guid.NewGuid().ToString();
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
        public string? Customer_MobileNumber { get; set; }
        public string? Card_Owner_Name { get; set; }
        public string? Customer_CreditCard_number { get; set; }
        public string? CreditCard_Bank { get; set; }
        public string? CreditCard_Operator { get; set; }
        public string? CreditCard_Catagory { get; set; }
        public string? CreditCard_Bill_Pay_Mode { get; set; }
        public decimal Bill_Amount_Pay { get; set; }
        public decimal Bill_Amount_Paid_fromuser { get; set; }
        public decimal Bill_Amount_Refund_Creditedd { get; set; }
        public decimal Gst { get; set; }
        public decimal Amount_Debited_fromuser { get; set; }
        public decimal Bill_Amount_Fetch { get; set; }
        public decimal Last_updatedbalance { get; set; }
        public decimal Description { get; set; }
        public string? Status { get; set; }
        public string? BBPS_Resopnce_Statuscode { get; set; } 
        public string? BBPS_TransactionId { get; set; } 
        public string? BBPS_Operator_Id { get; set; }
        public string? BBPS_status { get; set; }
        public DateTime Due_date { get; set; }
        public DateTime Transaction_created_At { get; set; } = DateTime.Now;
    }
}
