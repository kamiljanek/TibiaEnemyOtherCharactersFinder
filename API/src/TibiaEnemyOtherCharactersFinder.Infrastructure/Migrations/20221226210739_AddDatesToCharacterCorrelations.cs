using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaEnemyOtherCharactersFinder.Api.Migrations
{
    public partial class AddDatesToCharacterCorrelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "create_date",
                schema: "public",
                table: "character_correlations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2022, 12, 6));

            migrationBuilder.AddColumn<DateOnly>(
                name: "last_match_date",
                schema: "public",
                table: "character_correlations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2022, 12, 6));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "create_date",
                schema: "public",
                table: "character_correlations");

            migrationBuilder.DropColumn(
                name: "last_match_date",
                schema: "public",
                table: "character_correlations");
        }
    }
}
