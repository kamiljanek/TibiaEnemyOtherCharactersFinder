using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaCharFinder.Migrations
{
    public partial class fix_WorldCorrelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Characters_CharacterId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "Scans");

            migrationBuilder.DropIndex(
                name: "IX_Characters_CharacterId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "Characters");

            migrationBuilder.CreateTable(
                name: "WorldCorrelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    PossibleOtherCharacterId = table.Column<int>(type: "int", nullable: false),
                    LogInOrLogOutDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_WorldCorrelations_CharacterId",
                table: "WorldCorrelations",
                column: "CharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorldCorrelations");

            migrationBuilder.AddColumn<int>(
                name: "CharacterId",
                table: "Characters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Scans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharactersOnline = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScanCreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scans", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CharacterId",
                table: "Characters",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Characters_CharacterId",
                table: "Characters",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");
        }
    }
}
