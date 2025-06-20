using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newportal.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class bhjsbd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Useravailableservices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Useravailableservices");
        }
    }
}
