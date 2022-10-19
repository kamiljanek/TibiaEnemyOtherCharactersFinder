using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaCharacterFinderAPI.Migrations
{
    public partial class Change_CharacterLogoutOrLogin_column_type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterLogoutOrLogins_Characters_CharacterId",
                table: "CharacterLogoutOrLogins");

            migrationBuilder.DropIndex(
                name: "IX_CharacterLogoutOrLogins_CharacterId",
                table: "CharacterLogoutOrLogins");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "CharacterLogoutOrLogins");

            migrationBuilder.AddColumn<string>(
                name: "CharacterName",
                table: "CharacterLogoutOrLogins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CharacterName",
                table: "CharacterLogoutOrLogins");

            migrationBuilder.AddColumn<int>(
                name: "CharacterId",
                table: "CharacterLogoutOrLogins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLogoutOrLogins_CharacterId",
                table: "CharacterLogoutOrLogins",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterLogoutOrLogins_Characters_CharacterId",
                table: "CharacterLogoutOrLogins",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
