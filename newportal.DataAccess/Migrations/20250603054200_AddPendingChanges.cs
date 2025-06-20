using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newportal.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditReport",
                table: "UserCommissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CreditReport",
                table: "UserCommissions",
                type: "decimal(5,3)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
