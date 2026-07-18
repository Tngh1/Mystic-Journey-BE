using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddGolemBossAndGemReward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GemReward",
                table: "Monsters",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "ItemId", "BaseValue", "CorruptionReduction", "CreatedAt", "Description", "IconUrl", "IsActive", "MaxStack", "Name", "Rarity", "Slot", "Type" },
                values: new object[,]
                {
                    { 905, 800m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Heavy stone gloves dropped by GolemBoss.", null, true, 1, "Golem Boss Gloves", "Legendary", "Gloves", "Armor" },
                    { 906, 1000m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A massive stone armor dropped by GolemBoss.", null, true, 1, "Golem Boss Armor", "Legendary", "Armor", "Armor" }
                });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 1,
                column: "GemReward",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 2,
                column: "GemReward",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 3,
                column: "GemReward",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 4,
                column: "GemReward",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 5,
                column: "GemReward",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 6,
                column: "GemReward",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 7,
                column: "GemReward",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 8,
                column: "GemReward",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 9,
                column: "GemReward",
                value: 0m);

            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "MonsterId", "Atk", "AttackSpeed", "CreatedAt", "CritDamage", "CritRate", "Def", "Description", "ExperienceReward", "GemReward", "GoldReward", "ImageUrl", "IsActive", "Level", "MaxHp", "MoveSpeed", "Name", "Type" },
                values: new object[] { 10, 150, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15, 30, 70, "A giant stone golem boss.", 1500, 10m, 2000m, null, true, 15, 3000, 3, "GolemBoss", "Boss" });

            migrationBuilder.InsertData(
                table: "EquipmentStats",
                columns: new[] { "EquipmentStatsId", "BaseAtk", "BaseDef", "BaseHp", "BonusAtk", "BonusAttackSpeed", "BonusCritDamage", "BonusCritRate", "BonusDamageBonus", "BonusDef", "BonusHp", "BonusMoveSpeed", "ItemId" },
                values: new object[,]
                {
                    { 905, 50, 50, 200, 0, 0, 5, 5, 0, 0, 0, 0, 905 },
                    { 906, 0, 200, 1000, 0, 0, 0, 0, 0, 0, 0, 0, 906 }
                });

            migrationBuilder.InsertData(
                table: "MonsterDrops",
                columns: new[] { "MonsterDropId", "DropRate", "IsActive", "IsGuaranteed", "ItemId", "MaxQuantity", "MinQuantity", "MonsterId" },
                values: new object[,]
                {
                    { 905, 100.0, true, true, 905, 1, 1, 10 },
                    { 906, 100.0, true, true, 906, 1, 1, 10 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 905);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 906);

            migrationBuilder.DeleteData(
                table: "MonsterDrops",
                keyColumn: "MonsterDropId",
                keyValue: 905);

            migrationBuilder.DeleteData(
                table: "MonsterDrops",
                keyColumn: "MonsterDropId",
                keyValue: 906);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 905);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 906);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 10);

            migrationBuilder.DropColumn(
                name: "GemReward",
                table: "Monsters");
        }
    }
}
