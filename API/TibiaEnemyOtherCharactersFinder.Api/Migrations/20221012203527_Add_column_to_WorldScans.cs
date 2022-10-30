using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaCharacterFinderAPI.Migrations
{
    public partial class Add_column_to_WorldScans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorldScans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorldScans");
        }
    }
}
