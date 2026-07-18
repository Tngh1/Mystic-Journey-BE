using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddGoblinWarrior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 1,
                column: "CritDamage",
                value: 187);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 2,
                column: "CritDamage",
                value: 130);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 3,
                column: "CritDamage",
                value: 183);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 4,
                column: "CritDamage",
                value: 180);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 5,
                column: "CritDamage",
                value: 156);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 6,
                column: "CritDamage",
                value: 160);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 7,
                column: "CritDamage",
                value: 250);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 8,
                column: "CritDamage",
                value: 160);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 9,
                column: "CritDamage",
                value: 170);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 10,
                column: "CritDamage",
                value: 150);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 11,
                column: "CritDamage",
                value: 166);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 12,
                column: "CritDamage",
                value: 164);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 13,
                column: "CritDamage",
                value: 163);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 14,
                column: "CritDamage",
                value: 185);

            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "MonsterId", "Atk", "AttackSpeed", "CreatedAt", "CritDamage", "CritRate", "Def", "Description", "ExperienceReward", "GemReward", "GoldReward", "ImageUrl", "IsActive", "Level", "MaxHp", "MoveSpeed", "Name", "Type" },
                values: new object[] { 17, 70, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 20, 80, "A strong goblin warrior.", 50, 0m, 100m, null, true, 6, 450, 3, "GoblinWarrior", "Boss" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 17);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 1,
                column: "CritDamage",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 2,
                column: "CritDamage",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 3,
                column: "CritDamage",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 4,
                column: "CritDamage",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 5,
                column: "CritDamage",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 6,
                column: "CritDamage",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 7,
                column: "CritDamage",
                value: 25);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 8,
                column: "CritDamage",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 9,
                column: "CritDamage",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 10,
                column: "CritDamage",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 11,
                column: "CritDamage",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 12,
                column: "CritDamage",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 13,
                column: "CritDamage",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 14,
                column: "CritDamage",
                value: 12);
        }
    }
}
