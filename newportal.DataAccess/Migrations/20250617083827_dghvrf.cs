using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newportal.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class dghvrf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RechargeTransactions",
                columns: table => new
                {
                    Recharge_transaction_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Initiated_UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initiated_companyname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initiated_UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNumber_operator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Last_updatedbalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Circle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RechargeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RechargeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    debited_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BBPS_TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BBPS_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RechargeTransactions", x => x.Recharge_transaction_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RechargeTransactions");
        }
    }
}
