using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TibiaCharFinder.Migrations
{
    public partial class removeDateTimePropertyFromWorldCorrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogInOrLogOutDateTime",
                table: "WorldCorrelations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LogInOrLogOutDateTime",
                table: "WorldCorrelations",
                type: "datetime2",
                nullable: true);
        }
    }
}
