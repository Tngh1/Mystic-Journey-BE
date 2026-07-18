using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddOrcWarlord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "MonsterId", "Atk", "AttackSpeed", "CreatedAt", "CritDamage", "CritRate", "Def", "Description", "ExperienceReward", "GemReward", "GoldReward", "ImageUrl", "IsActive", "Level", "MaxHp", "MoveSpeed", "Name", "Type" },
                values: new object[] { 20, 100, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 175, 28, 80, "A formidable orc warlord.", 55, 0m, 100m, null, true, 7, 600, 3, "OrcWarlord", "Normal" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 20);
        }
    }
}
