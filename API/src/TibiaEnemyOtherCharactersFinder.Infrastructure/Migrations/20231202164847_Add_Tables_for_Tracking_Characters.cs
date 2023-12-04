using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesforTrackingCharacters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "url",
                schema: "public",
                table: "worlds",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "public",
                table: "worlds",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "public",
                table: "characters",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "character_name",
                schema: "public",
                table: "character_actions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "online_characters",
                schema: "public",
                columns: table => new
                {
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    worldname = table.Column<string>(name: "world_name", type: "character varying(20)", maxLength: 20, nullable: false),
                    onlinedatetime = table.Column<DateTime>(name: "online_date_time", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_online_characters", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "tracked_characters",
                schema: "public",
                columns: table => new
                {
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    connectionid = table.Column<string>(name: "connection_id", type: "character varying(100)", maxLength: 100, nullable: false),
                    worldname = table.Column<string>(name: "world_name", type: "character varying(20)", maxLength: 20, nullable: false),
                    starttrackdatetime = table.Column<DateTime>(name: "start_track_date_time", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tracked_characters", x => new { x.name, x.connectionid });
                });

            migrationBuilder.CreateIndex(
                name: "ix_online_characters_name",
                schema: "public",
                table: "online_characters",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_online_characters_world_name",
                schema: "public",
                table: "online_characters",
                column: "world_name");

            migrationBuilder.CreateIndex(
                name: "ix_tracked_characters_name",
                schema: "public",
                table: "tracked_characters",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_tracked_characters_world_name",
                schema: "public",
                table: "tracked_characters",
                column: "world_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "online_characters",
                schema: "public");

            migrationBuilder.DropTable(
                name: "tracked_characters",
                schema: "public");

            migrationBuilder.AlterColumn<string>(
                name: "url",
                schema: "public",
                table: "worlds",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "public",
                table: "worlds",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "public",
                table: "characters",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "character_name",
                schema: "public",
                table: "character_actions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
