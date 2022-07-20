using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaCharFinder.Migrations
{
    public partial class addSecondRelationBetweenCharacterAndLoginWorldCorrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorldCorrelations_Characters_CharacterId",
                table: "WorldCorrelations");

            migrationBuilder.RenameColumn(
                name: "PossibleOtherCharacterId",
                table: "WorldCorrelations",
                newName: "LogoutCharacterId");

            migrationBuilder.RenameColumn(
                name: "CharacterId",
                table: "WorldCorrelations",
                newName: "LoginCharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_WorldCorrelations_CharacterId",
                table: "WorldCorrelations",
                newName: "IX_WorldCorrelations_LoginCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_WorldCorrelations_LogoutCharacterId",
                table: "WorldCorrelations",
                column: "LogoutCharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorldCorrelations_Characters_LoginCharacterId",
                table: "WorldCorrelations",
                column: "LoginCharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorldCorrelations_Characters_LogoutCharacterId",
                table: "WorldCorrelations",
                column: "LogoutCharacterId",
                principalTable: "Characters",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorldCorrelations_Characters_LoginCharacterId",
                table: "WorldCorrelations");

            migrationBuilder.DropForeignKey(
                name: "FK_WorldCorrelations_Characters_LogoutCharacterId",
                table: "WorldCorrelations");

            migrationBuilder.DropIndex(
                name: "IX_WorldCorrelations_LogoutCharacterId",
                table: "WorldCorrelations");

            migrationBuilder.RenameColumn(
                name: "LogoutCharacterId",
                table: "WorldCorrelations",
                newName: "PossibleOtherCharacterId");

            migrationBuilder.RenameColumn(
                name: "LoginCharacterId",
                table: "WorldCorrelations",
                newName: "CharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_WorldCorrelations_LoginCharacterId",
                table: "WorldCorrelations",
                newName: "IX_WorldCorrelations_CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorldCorrelations_Characters_CharacterId",
                table: "WorldCorrelations",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
