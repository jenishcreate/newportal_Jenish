using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newportal.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class xjnbfj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PG1",
                table: "UserCommissions",
                type: "decimal(5,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PG2",
                table: "UserCommissions",
                type: "decimal(5,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PG3",
                table: "UserCommissions",
                type: "decimal(5,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PG4",
                table: "UserCommissions",
                type: "decimal(5,3)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PG1",
                table: "UserCommissions");

            migrationBuilder.DropColumn(
                name: "PG2",
                table: "UserCommissions");

            migrationBuilder.DropColumn(
                name: "PG3",
                table: "UserCommissions");

            migrationBuilder.DropColumn(
                name: "PG4",
                table: "UserCommissions");
        }
    }
}
