using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaEnemyOtherCharactersFinder.Api.Migrations
{
    public partial class AddLogoutOrLoginDate_to_CharacterActions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "logout_or_login_date",
                schema: "public",
                table: "character_actions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "logout_or_login_date",
                schema: "public",
                table: "character_actions");
        }
    }
}
