using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaCharacterFinderAPI.Migrations
{
    public partial class ChangeTableNameCharacterCorrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterCorrelationInSpecificWorld");

            migrationBuilder.CreateTable(
                name: "CharacterCorrelations",
                columns: table => new
                {
                    CorrelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogoutCharacterId = table.Column<int>(type: "int", nullable: false),
                    LoginCharacterId = table.Column<int>(type: "int", nullable: false),
                    NumberOfMatches = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterCorrelations", x => x.CorrelationId);
                    table.ForeignKey(
                        name: "FK_CharacterCorrelations_Characters_LoginCharacterId",
                        column: x => x.LoginCharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId");
                    table.ForeignKey(
                        name: "FK_CharacterCorrelations_Characters_LogoutCharacterId",
                        column: x => x.LogoutCharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterCorrelations_LoginCharacterId",
                table: "CharacterCorrelations",
                column: "LoginCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterCorrelations_LogoutCharacterId",
                table: "CharacterCorrelations",
                column: "LogoutCharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterCorrelations");

            migrationBuilder.CreateTable(
                name: "CharacterCorrelationInSpecificWorld",
                columns: table => new
                {
                    CorrelationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginCharacterId = table.Column<int>(type: "int", nullable: false),
                    LogoutCharacterId = table.Column<int>(type: "int", nullable: false),
                    NumberOfMatches = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterCorrelationInSpecificWorld", x => x.CorrelationId);
                    table.ForeignKey(
                        name: "FK_CharacterCorrelationInSpecificWorld_Characters_LoginCharacterId",
                        column: x => x.LoginCharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId");
                    table.ForeignKey(
                        name: "FK_CharacterCorrelationInSpecificWorld_Characters_LogoutCharacterId",
                        column: x => x.LogoutCharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterCorrelationInSpecificWorld_LoginCharacterId",
                table: "CharacterCorrelationInSpecificWorld",
                column: "LoginCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterCorrelationInSpecificWorld_LogoutCharacterId",
                table: "CharacterCorrelationInSpecificWorld",
                column: "LogoutCharacterId");
        }
    }
}
