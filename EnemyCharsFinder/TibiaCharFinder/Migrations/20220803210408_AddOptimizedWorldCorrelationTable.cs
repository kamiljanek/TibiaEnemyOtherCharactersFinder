using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaCharFinder.Migrations
{
    public partial class AddOptimizedWorldCorrelationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OptimizedWorldCorrelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogoutOrLoginCharacterId = table.Column<int>(type: "int", nullable: false),
                    PossibleOtherCharactersId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptimizedWorldCorrelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptimizedWorldCorrelations_Characters_LogoutOrLoginCharacterId",
                        column: x => x.LogoutOrLoginCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptimizedWorldCorrelations_LogoutOrLoginCharacterId",
                table: "OptimizedWorldCorrelations",
                column: "LogoutOrLoginCharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptimizedWorldCorrelations");
        }
    }
}
