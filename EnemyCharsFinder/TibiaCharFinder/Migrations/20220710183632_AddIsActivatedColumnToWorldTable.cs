using Microsoft.EntityFrameworkCore.Migrations;

namespace TibiaCharFinder.Migrations
{
    public partial class AddIsActivatedColumnToWorldTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActivated",
                table: "Worlds",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActivated",
                table: "Worlds");
        }
    }
}
