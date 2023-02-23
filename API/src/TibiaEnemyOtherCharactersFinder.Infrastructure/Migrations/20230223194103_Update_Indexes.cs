using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_characters_character_id",
                schema: "public",
                table: "characters",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "ix_characters_name",
                schema: "public",
                table: "characters",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_character_actions_character_name",
                schema: "public",
                table: "character_actions",
                column: "character_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_characters_character_id",
                schema: "public",
                table: "characters");

            migrationBuilder.DropIndex(
                name: "ix_characters_name",
                schema: "public",
                table: "characters");

            migrationBuilder.DropIndex(
                name: "ix_character_actions_character_name",
                schema: "public",
                table: "character_actions");
        }
    }
}
