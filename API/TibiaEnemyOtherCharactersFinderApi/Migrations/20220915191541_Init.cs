using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaCharacterFinderAPI.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Worlds",
                columns: table => new
                {
                    WorldId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(15)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worlds", x => x.WorldId);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    WorldId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_Characters_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "WorldId");
                });

            migrationBuilder.CreateTable(
                name: "WorldScans",
                columns: table => new
                {
                    WorldScanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharactersOnline = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorldId = table.Column<short>(type: "smallint", nullable: false),
                    ScanCreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldScans", x => x.WorldScanId);
                    table.ForeignKey(
                        name: "FK_WorldScans_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "WorldId");
                });

            migrationBuilder.CreateTable(
                name: "CharacterCorrelationInSpecificWorld",
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

            migrationBuilder.CreateTable(
                name: "CharacterLogoutOrLogins",
                columns: table => new
                {
                    CharacterLogoutOrLoginId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    WorldScanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterLogoutOrLogins", x => x.CharacterLogoutOrLoginId);
                    table.ForeignKey(
                        name: "FK_CharacterLogoutOrLogins_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterLogoutOrLogins_WorldScans_WorldScanId",
                        column: x => x.WorldScanId,
                        principalTable: "WorldScans",
                        principalColumn: "WorldScanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterCorrelationInSpecificWorld_LoginCharacterId",
                table: "CharacterCorrelationInSpecificWorld",
                column: "LoginCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterCorrelationInSpecificWorld_LogoutCharacterId",
                table: "CharacterCorrelationInSpecificWorld",
                column: "LogoutCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLogoutOrLogins_CharacterId",
                table: "CharacterLogoutOrLogins",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLogoutOrLogins_WorldScanId",
                table: "CharacterLogoutOrLogins",
                column: "WorldScanId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_WorldId",
                table: "Characters",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_WorldScans_WorldId",
                table: "WorldScans",
                column: "WorldId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterCorrelationInSpecificWorld");

            migrationBuilder.DropTable(
                name: "CharacterLogoutOrLogins");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "WorldScans");

            migrationBuilder.DropTable(
                name: "Worlds");
        }
    }
}
