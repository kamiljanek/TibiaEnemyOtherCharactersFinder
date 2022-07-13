using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaCharFinder.Migrations
{
    public partial class a : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Correlations");

            migrationBuilder.DropTable(
                name: "WorldCorrelations");

            migrationBuilder.AddColumn<int>(
                name: "CharacterId",
                table: "Characters",
                type: "int",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Characters_CharacterId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_CharacterId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "Characters");

            migrationBuilder.CreateTable(
                name: "Correlations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    LogInOrLogOutDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PossibleCharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Correlations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Correlations_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorldCorrelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    LogInOrLogOutDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_Correlations_CharacterId",
                table: "Correlations",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_WorldCorrelations_CharacterId",
                table: "WorldCorrelations",
                column: "CharacterId");
        }
    }
}
