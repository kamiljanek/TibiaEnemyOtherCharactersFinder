using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaCharacterFinderAPI.Migrations
{
    public partial class AddColumnWorldId_To_CharacterLogoutOrLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "WorldId",
                table: "CharacterLogoutOrLogins",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLogoutOrLogins_WorldId",
                table: "CharacterLogoutOrLogins",
                column: "WorldId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterLogoutOrLogins_Worlds_WorldId",
                table: "CharacterLogoutOrLogins",
                column: "WorldId",
                principalTable: "Worlds",
                principalColumn: "WorldId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterLogoutOrLogins_Worlds_WorldId",
                table: "CharacterLogoutOrLogins");

            migrationBuilder.DropIndex(
                name: "IX_CharacterLogoutOrLogins_WorldId",
                table: "CharacterLogoutOrLogins");

            migrationBuilder.DropColumn(
                name: "WorldId",
                table: "CharacterLogoutOrLogins");
        }
    }
}
