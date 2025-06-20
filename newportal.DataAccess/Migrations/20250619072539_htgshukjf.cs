using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newportal.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class htgshukjf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Description",
                table: "WalletTransaction",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "BBPS_Operator_Id",
                table: "RechargeTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BBPS_Resopnce_Statuscode",
                table: "RechargeTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Bill_Amount_Fetch",
                table: "RechargeTransactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RechargeTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Gst",
                table: "RechargeTransactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Initiated_User_IP",
                table: "RechargeTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Initiated_User_L2_ParentCompany",
                table: "RechargeTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Initiated_User_L2_Parent_Number",
                table: "RechargeTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Initiated_User_L2_Parentid",
                table: "RechargeTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Initiated_User_ParentCompany",
                table: "RechargeTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Initiated_User_Parent_Number",
                table: "RechargeTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Initiated_User_Parentid",
                table: "RechargeTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CreditCardBillTransactions",
                columns: table => new
                {
                    CreditcardBillTransactionid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Initiated_UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initiated_User_IP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initiated_Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initiated_MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initiated_companyname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initiated_User_Parent_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initiated_User_Parentid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initiated_User_ParentCompany = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initiated_User_L2_Parent_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initiated_User_L2_Parentid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initiated_User_L2_ParentCompany = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Card_Owner_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_CreditCard_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditCard_Bank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditCard_Operator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditCard_Catodary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditCard_Bill_Pay_Mode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bill_Amount_Pay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bill_Amount_Paid_fromuser = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bill_Amount_Refund_Creditedd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Gst = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount_Debited_fromuser = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bill_Amount_Fetch = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Last_updatedbalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BBPS_Resopnce_Statuscode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BBPS_TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BBPS_Operator_Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BBPS_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Due_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Transaction_created_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardBillTransactions", x => x.CreditcardBillTransactionid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditCardBillTransactions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "WalletTransaction");

            migrationBuilder.DropColumn(
                name: "BBPS_Operator_Id",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "BBPS_Resopnce_Statuscode",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "Bill_Amount_Fetch",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "Gst",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "Initiated_User_IP",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "Initiated_User_L2_ParentCompany",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "Initiated_User_L2_Parent_Number",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "Initiated_User_L2_Parentid",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "Initiated_User_ParentCompany",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "Initiated_User_Parent_Number",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "Initiated_User_Parentid",
                table: "RechargeTransactions");
        }
    }
}
