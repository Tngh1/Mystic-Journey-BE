using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddOgre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 16,
                column: "Type",
                value: "Normal");

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 17,
                column: "Type",
                value: "Normal");

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 18,
                column: "Type",
                value: "Normal");

            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "MonsterId", "Atk", "AttackSpeed", "CreatedAt", "CritDamage", "CritRate", "Def", "Description", "ExperienceReward", "GemReward", "GoldReward", "ImageUrl", "IsActive", "Level", "MaxHp", "MoveSpeed", "Name", "Type" },
                values: new object[] { 19, 50, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 20, 40, "A fierce ogre.", 50, 0m, 100m, null, true, 6, 450, 3, "Ogre", "Normal" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 19);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 16,
                column: "Type",
                value: "Boss");

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 17,
                column: "Type",
                value: "Boss");

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 18,
                column: "Type",
                value: "Boss");
        }
    }
}
