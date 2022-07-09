using Microsoft.EntityFrameworkCore.Migrations;

namespace TibiaCharFinder.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Correlations_CorrelationId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_CorrelationId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "Characters");

            migrationBuilder.AddColumn<string>(
                name: "PossibleOtherCharacters",
                table: "Correlations",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PossibleOtherCharacters",
                table: "Correlations");

            migrationBuilder.AddColumn<int>(
                name: "CorrelationId",
                table: "Characters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CorrelationId",
                table: "Characters",
                column: "CorrelationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Correlations_CorrelationId",
                table: "Characters",
                column: "CorrelationId",
                principalTable: "Correlations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
