using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFoundInScanToCharacters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "found_in_scan",
                schema: "public",
                table: "characters",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "found_in_scan",
                schema: "public",
                table: "characters");
        }
    }
}
