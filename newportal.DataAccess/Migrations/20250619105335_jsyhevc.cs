using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newportal.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class jsyhevc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreditCard_Catodary",
                table: "CreditCardBillTransactions",
                newName: "CreditCard_Catagory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreditCard_Catagory",
                table: "CreditCardBillTransactions",
                newName: "CreditCard_Catodary");
        }
    }
}
