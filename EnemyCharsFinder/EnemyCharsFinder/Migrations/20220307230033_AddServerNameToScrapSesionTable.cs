using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnemyCharsFinder.Migrations
{
    public partial class AddServerNameToScrapSesionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Names",
                table: "ScrapSesions",
                newName: "ServerName");

            migrationBuilder.AddColumn<string>(
                name: "OnlineCharacterNames",
                table: "ScrapSesions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlineCharacterNames",
                table: "ScrapSesions");

            migrationBuilder.RenameColumn(
                name: "ServerName",
                table: "ScrapSesions",
                newName: "Names");
        }
    }
}
