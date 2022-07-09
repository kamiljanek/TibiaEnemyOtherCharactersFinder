using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TibiaCharFinder.Migrations
{
    public partial class CorrelationCharacterAndScanColumnAdjustment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScanWorlds");

            migrationBuilder.DropColumn(
                name: "PossibleOtherCharacters",
                table: "Correlations");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Worlds",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "PossibleCharacterId",
                table: "Correlations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WorldCorrelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    LogInOrLogOutDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PossibleCharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldCorrelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorldCorrelations_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorldScans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharactersOnline = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorldId = table.Column<int>(type: "int", nullable: false),
                    ScanCreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldScans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorldScans_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorldCorrelations_CharacterId",
                table: "WorldCorrelations",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_WorldScans_WorldId",
                table: "WorldScans",
                column: "WorldId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorldCorrelations");

            migrationBuilder.DropTable(
                name: "WorldScans");

            migrationBuilder.DropColumn(
                name: "PossibleCharacterId",
                table: "Correlations");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Worlds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PossibleOtherCharacters",
                table: "Correlations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ScanWorlds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharactersOnline = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScanCreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorldId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanWorlds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScanWorlds_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScanWorlds_WorldId",
                table: "ScanWorlds",
                column: "WorldId");
        }
    }
}
