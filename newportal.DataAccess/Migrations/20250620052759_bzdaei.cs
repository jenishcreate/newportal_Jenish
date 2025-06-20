using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newportal.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class bzdaei : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Initiated_User_number",
                table: "RechargeTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "User_Latitude",
                table: "RechargeTransactions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "User_Longitude",
                table: "RechargeTransactions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "User_Latitude",
                table: "CreditCardBillTransactions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "User_Longitude",
                table: "CreditCardBillTransactions",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Initiated_User_number",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "User_Latitude",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "User_Longitude",
                table: "RechargeTransactions");

            migrationBuilder.DropColumn(
                name: "User_Latitude",
                table: "CreditCardBillTransactions");

            migrationBuilder.DropColumn(
                name: "User_Longitude",
                table: "CreditCardBillTransactions");
        }
    }
}
