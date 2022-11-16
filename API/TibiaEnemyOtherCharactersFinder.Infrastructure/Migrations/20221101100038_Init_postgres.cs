using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TibiaEnemyOtherCharactersFinder.Api.Migrations
{
    public partial class Init_postgres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "worlds",
                schema: "public",
                columns: table => new
                {
                    world_id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(50)", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_worlds", x => x.world_id);
                });

            migrationBuilder.CreateTable(
                name: "characters",
                schema: "public",
                columns: table => new
                {
                    character_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(50)", nullable: false),
                    world_id = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characters", x => x.character_id);
                    table.ForeignKey(
                        name: "fk_characters_worlds_world_id",
                        column: x => x.world_id,
                        principalSchema: "public",
                        principalTable: "worlds",
                        principalColumn: "world_id");
                });

            migrationBuilder.CreateTable(
                name: "world_scans",
                schema: "public",
                columns: table => new
                {
                    world_scan_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    characters_online = table.Column<string>(type: "text", nullable: false),
                    world_id = table.Column<short>(type: "smallint", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    scan_create_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_world_scans", x => x.world_scan_id);
                    table.ForeignKey(
                        name: "fk_world_scans_worlds_world_id",
                        column: x => x.world_id,
                        principalSchema: "public",
                        principalTable: "worlds",
                        principalColumn: "world_id");
                });

            migrationBuilder.CreateTable(
                name: "character_correlations",
                schema: "public",
                columns: table => new
                {
                    correlation_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    logout_character_id = table.Column<int>(type: "integer", nullable: false),
                    login_character_id = table.Column<int>(type: "integer", nullable: false),
                    number_of_matches = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_correlations", x => x.correlation_id);
                    table.ForeignKey(
                        name: "fk_character_correlations_characters_character_id",
                        column: x => x.logout_character_id,
                        principalSchema: "public",
                        principalTable: "characters",
                        principalColumn: "character_id");
                    table.ForeignKey(
                        name: "fk_character_correlations_characters_login_character_id",
                        column: x => x.login_character_id,
                        principalSchema: "public",
                        principalTable: "characters",
                        principalColumn: "character_id");
                });

            migrationBuilder.CreateTable(
                name: "character_actions",
                schema: "public",
                columns: table => new
                {
                    character_action_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    character_name = table.Column<string>(type: "varchar(50)", nullable: false),
                    world_scan_id = table.Column<int>(type: "integer", nullable: false),
                    is_online = table.Column<bool>(type: "boolean", nullable: false),
                    world_id = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_actions", x => x.character_action_id);
                    table.ForeignKey(
                        name: "fk_character_actions_world_scans_world_scan_id",
                        column: x => x.world_scan_id,
                        principalSchema: "public",
                        principalTable: "world_scans",
                        principalColumn: "world_scan_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_character_actions_worlds_world_id",
                        column: x => x.world_id,
                        principalSchema: "public",
                        principalTable: "worlds",
                        principalColumn: "world_id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_character_actions_world_id",
                schema: "public",
                table: "character_actions",
                column: "world_id");

            migrationBuilder.CreateIndex(
                name: "ix_character_actions_world_scan_id",
                schema: "public",
                table: "character_actions",
                column: "world_scan_id");

            migrationBuilder.CreateIndex(
                name: "ix_character_correlations_login_character_id",
                schema: "public",
                table: "character_correlations",
                column: "login_character_id");

            migrationBuilder.CreateIndex(
                name: "ix_character_correlations_logout_character_id",
                schema: "public",
                table: "character_correlations",
                column: "logout_character_id");

            migrationBuilder.CreateIndex(
                name: "ix_characters_world_id",
                schema: "public",
                table: "characters",
                column: "world_id");

            migrationBuilder.CreateIndex(
                name: "ix_world_scans_world_id",
                schema: "public",
                table: "world_scans",
                column: "world_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_actions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "character_correlations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "world_scans",
                schema: "public");

            migrationBuilder.DropTable(
                name: "characters",
                schema: "public");

            migrationBuilder.DropTable(
                name: "worlds",
                schema: "public");
        }
    }
}
