using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVerifiedDateandTradedDateColumntoCharacter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_character_correlations_characters_character_id",
                schema: "public",
                table: "character_correlations");

            migrationBuilder.DropForeignKey(
                name: "fk_character_correlations_characters_login_character_id",
                schema: "public",
                table: "character_correlations");

            migrationBuilder.AddColumn<DateOnly>(
                name: "traded_date",
                schema: "public",
                table: "characters",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "verified_date",
                schema: "public",
                table: "characters",
                type: "date",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_character_correlations_characters_character_id",
                schema: "public",
                table: "character_correlations",
                column: "logout_character_id",
                principalSchema: "public",
                principalTable: "characters",
                principalColumn: "character_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_character_correlations_characters_login_character_id",
                schema: "public",
                table: "character_correlations",
                column: "login_character_id",
                principalSchema: "public",
                principalTable: "characters",
                principalColumn: "character_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_character_correlations_characters_character_id",
                schema: "public",
                table: "character_correlations");

            migrationBuilder.DropForeignKey(
                name: "fk_character_correlations_characters_login_character_id",
                schema: "public",
                table: "character_correlations");

            migrationBuilder.DropColumn(
                name: "traded_date",
                schema: "public",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "verified_date",
                schema: "public",
                table: "characters");

            migrationBuilder.AddForeignKey(
                name: "fk_character_correlations_characters_character_id",
                schema: "public",
                table: "character_correlations",
                column: "logout_character_id",
                principalSchema: "public",
                principalTable: "characters",
                principalColumn: "character_id");

            migrationBuilder.AddForeignKey(
                name: "fk_character_correlations_characters_login_character_id",
                schema: "public",
                table: "character_correlations",
                column: "login_character_id",
                principalSchema: "public",
                principalTable: "characters",
                principalColumn: "character_id");
        }
    }
}
