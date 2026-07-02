using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDungeonSessionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClaimedAt",
                table: "DungeonSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BossSpawned",
                table: "DungeonProgresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ElapsedTime",
                table: "DungeonProgresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimedAt",
                table: "DungeonSessions");

            migrationBuilder.DropColumn(
                name: "BossSpawned",
                table: "DungeonProgresses");

            migrationBuilder.DropColumn(
                name: "ElapsedTime",
                table: "DungeonProgresses");
        }
    }
}
