using Microsoft.EntityFrameworkCore.Migrations;

namespace TibiaCharFinder.Migrations
{
    public partial class ChangeIsActivatedColumnNameInWorldTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActivated",
                table: "Worlds",
                newName: "IsAvailable");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAvailable",
                table: "Worlds",
                newName: "IsActivated");
        }
    }
}
