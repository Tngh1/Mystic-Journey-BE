using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSwampDemonSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "ItemId", "BaseValue", "CorruptionReduction", "CreatedAt", "Description", "IconUrl", "IsActive", "MaxStack", "Name", "Rarity", "Slot", "Type" },
                values: new object[,]
                {
                    { 901, 100m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thanh kiếm sắc bén rớt ra từ SwampDemon.", null, true, 1, "Kiếm Đầm Lầy (Swamp Sword)", "Rare", "Weapon", "Weapon" },
                    { 902, 150m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chiếc áo giáp bền bỉ rớt ra từ SwampDemon.", null, true, 1, "Giáp Đầm Lầy (Swamp Armor)", "Rare", "Armor", "Armor" }
                });

            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "MonsterId", "Atk", "AttackSpeed", "CreatedAt", "CritDamage", "CritRate", "Def", "Description", "ExperienceReward", "GoldReward", "ImageUrl", "IsActive", "Level", "MaxHp", "MoveSpeed", "Name", "Type" },
                values: new object[] { 2, 20, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 20, 10, "Quái vật đầm lầy nguy hiểm.", 100, 200m, null, true, 10, 500, 1, "SwampDemon", "Boss" });

            migrationBuilder.InsertData(
                table: "EquipmentStats",
                columns: new[] { "EquipmentStatsId", "BaseAtk", "BaseDef", "BaseHp", "BonusAtk", "BonusAttackSpeed", "BonusCritDamage", "BonusCritRate", "BonusDamageBonus", "BonusDef", "BonusHp", "BonusMoveSpeed", "ItemId" },
                values: new object[,]
                {
                    { 901, 15, 0, 0, 0, 0, 10, 5, 0, 0, 0, 0, 901 },
                    { 902, 0, 20, 100, 0, 0, 0, 0, 0, 0, 0, 0, 902 }
                });

            migrationBuilder.InsertData(
                table: "MonsterDrops",
                columns: new[] { "MonsterDropId", "DropRate", "IsActive", "IsGuaranteed", "ItemId", "MaxQuantity", "MinQuantity", "MonsterId" },
                values: new object[,]
                {
                    { 901, 100.0, true, true, 901, 1, 1, 2 },
                    { 902, 100.0, true, true, 902, 1, 1, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 901);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 902);

            migrationBuilder.DeleteData(
                table: "MonsterDrops",
                keyColumn: "MonsterDropId",
                keyValue: 901);

            migrationBuilder.DeleteData(
                table: "MonsterDrops",
                keyColumn: "MonsterDropId",
                keyValue: 902);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 901);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 902);

            migrationBuilder.DeleteData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 2);
        }
    }
}
