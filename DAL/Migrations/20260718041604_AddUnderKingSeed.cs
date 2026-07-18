using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUnderKingSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "ItemId", "BaseValue", "CorruptionReduction", "CreatedAt", "Description", "IconUrl", "IsActive", "MaxStack", "Name", "Rarity", "Slot", "Type" },
                values: new object[,]
                {
                    { 907, 1500m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A royal cursed sword dropped by UnderKing.", null, true, 1, "UnderKing Sword", "Legendary", "Weapon", "Weapon" },
                    { 908, 2000m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The crown of the UnderKing.", null, true, 1, "UnderKing Crown", "Legendary", "Helmet", "Armor" }
                });

            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "MonsterId", "Atk", "AttackSpeed", "CreatedAt", "CritDamage", "CritRate", "Def", "Description", "ExperienceReward", "GemReward", "GoldReward", "ImageUrl", "IsActive", "Level", "MaxHp", "MoveSpeed", "Name", "Type" },
                values: new object[] { 15, 200, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20, 40, 300, "The supreme skeleton king.", 900, 500m, 2500m, null, true, 20, 10000, 4, "UnderKing", "Boss" });

            migrationBuilder.InsertData(
                table: "EquipmentStats",
                columns: new[] { "EquipmentStatsId", "BaseAtk", "BaseDef", "BaseHp", "BonusAtk", "BonusAttackSpeed", "BonusCritDamage", "BonusCritRate", "BonusDamageBonus", "BonusDef", "BonusHp", "BonusMoveSpeed", "ItemId" },
                values: new object[,]
                {
                    { 907, 200, 0, 0, 0, 0, 20, 20, 0, 0, 0, 0, 907 },
                    { 908, 50, 300, 1000, 0, 0, 10, 10, 0, 0, 0, 0, 908 }
                });

            migrationBuilder.InsertData(
                table: "MonsterDrops",
                columns: new[] { "MonsterDropId", "DropRate", "IsActive", "IsGuaranteed", "ItemId", "MaxQuantity", "MinQuantity", "MonsterId" },
                values: new object[,]
                {
                    { 907, 100.0, true, true, 907, 1, 1, 15 },
                    { 908, 100.0, true, true, 908, 1, 1, 15 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 907);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 908);

            migrationBuilder.DeleteData(
                table: "MonsterDrops",
                keyColumn: "MonsterDropId",
                keyValue: 907);

            migrationBuilder.DeleteData(
                table: "MonsterDrops",
                keyColumn: "MonsterDropId",
                keyValue: 908);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 907);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 908);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 15);
        }
    }
}
