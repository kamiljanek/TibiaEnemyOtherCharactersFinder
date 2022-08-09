using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaCharFinder.Migrations
{
    public partial class Remove_Old_WorldCorrelations_And_Turn_OptimizedWorldCorrelation_To_WorldCorrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorldCorrelations_Characters_LoginCharacterId",
                table: "WorldCorrelations");

            migrationBuilder.DropForeignKey(
                name: "FK_WorldCorrelations_Characters_LogoutCharacterId",
                table: "WorldCorrelations");

            migrationBuilder.DropTable(
                name: "OptimizedWorldCorrelations");

            migrationBuilder.DropIndex(
                name: "IX_WorldCorrelations_LoginCharacterId",
                table: "WorldCorrelations");

            migrationBuilder.DropColumn(
                name: "LoginCharacterId",
                table: "WorldCorrelations");

            migrationBuilder.RenameColumn(
                name: "LogoutCharacterId",
                table: "WorldCorrelations",
                newName: "CharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_WorldCorrelations_LogoutCharacterId",
                table: "WorldCorrelations",
                newName: "IX_WorldCorrelations_CharacterId");

            migrationBuilder.AddColumn<string>(
                name: "PossibleOtherCharactersId",
                table: "WorldCorrelations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_WorldCorrelations_Characters_CharacterId",
                table: "WorldCorrelations",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorldCorrelations_Characters_CharacterId",
                table: "WorldCorrelations");

            migrationBuilder.DropColumn(
                name: "PossibleOtherCharactersId",
                table: "WorldCorrelations");

            migrationBuilder.RenameColumn(
                name: "CharacterId",
                table: "WorldCorrelations",
                newName: "LogoutCharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_WorldCorrelations_CharacterId",
                table: "WorldCorrelations",
                newName: "IX_WorldCorrelations_LogoutCharacterId");

            migrationBuilder.AddColumn<int>(
                name: "LoginCharacterId",
                table: "WorldCorrelations",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                name: "IX_WorldCorrelations_LoginCharacterId",
                table: "WorldCorrelations",
                column: "LoginCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_OptimizedWorldCorrelations_LogoutOrLoginCharacterId",
                table: "OptimizedWorldCorrelations",
                column: "LogoutOrLoginCharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorldCorrelations_Characters_LoginCharacterId",
                table: "WorldCorrelations",
                column: "LoginCharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorldCorrelations_Characters_LogoutCharacterId",
                table: "WorldCorrelations",
                column: "LogoutCharacterId",
                principalTable: "Characters",
                principalColumn: "Id");
        }
    }
}
