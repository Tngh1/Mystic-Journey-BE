using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDragonBossIdleSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "ItemId", "BaseValue", "CorruptionReduction", "CreatedAt", "Description", "IconUrl", "IsActive", "MaxStack", "Name", "Rarity", "Slot", "Type" },
                values: new object[,]
                {
                    { 903, 500m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A legendary sword dropped by DragonBossIdle.", null, true, 1, "Dragon Boss Sword", "Legendary", "Weapon", "Weapon" },
                    { 904, 600m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A legendary armor dropped by DragonBossIdle.", null, true, 1, "Dragon Boss Armor", "Legendary", "Armor", "Armor" }
                });

            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "MonsterId", "Atk", "AttackSpeed", "CreatedAt", "CritDamage", "CritRate", "Def", "Description", "ExperienceReward", "GoldReward", "ImageUrl", "IsActive", "Level", "MaxHp", "MoveSpeed", "Name", "Type" },
                values: new object[] { 7, 50, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 25, 30, 35, "A terrifying boss dragon.", 300, 1000m, null, true, 20, 1000, 0, "DragonBossIdle", "Boss" });

            migrationBuilder.InsertData(
                table: "EquipmentStats",
                columns: new[] { "EquipmentStatsId", "BaseAtk", "BaseDef", "BaseHp", "BonusAtk", "BonusAttackSpeed", "BonusCritDamage", "BonusCritRate", "BonusDamageBonus", "BonusDef", "BonusHp", "BonusMoveSpeed", "ItemId" },
                values: new object[,]
                {
                    { 903, 100, 0, 0, 0, 0, 20, 15, 0, 0, 0, 0, 903 },
                    { 904, 0, 100, 500, 0, 0, 0, 0, 0, 0, 0, 0, 904 }
                });

            migrationBuilder.InsertData(
                table: "MonsterDrops",
                columns: new[] { "MonsterDropId", "DropRate", "IsActive", "IsGuaranteed", "ItemId", "MaxQuantity", "MinQuantity", "MonsterId" },
                values: new object[,]
                {
                    { 903, 100.0, true, true, 903, 1, 1, 7 },
                    { 904, 100.0, true, true, 904, 1, 1, 7 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 903);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 904);

            migrationBuilder.DeleteData(
                table: "MonsterDrops",
                keyColumn: "MonsterDropId",
                keyValue: 903);

            migrationBuilder.DeleteData(
                table: "MonsterDrops",
                keyColumn: "MonsterDropId",
                keyValue: 904);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 903);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 904);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 7);
        }
    }
}
