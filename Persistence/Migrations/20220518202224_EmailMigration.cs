using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageTrackerAPI.Persistence.Migrations
{
    public partial class EmailMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenderEmail",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderEmail",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "Packages");
        }
    }
}
