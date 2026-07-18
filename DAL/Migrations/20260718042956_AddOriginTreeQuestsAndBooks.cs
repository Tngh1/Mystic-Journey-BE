using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginTreeQuestsAndBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "ItemId", "BaseValue", "CorruptionReduction", "CreatedAt", "Description", "IconUrl", "IsActive", "MaxStack", "Name", "Rarity", "Slot", "Type" },
                values: new object[,]
                {
                    { 909, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A magic book containing the power to seal the Origin Tree, guarded by SwampDemon.", null, true, 1, "Swamp Seal Book", "Legendary", "None", "QuestItem" },
                    { 910, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A magic book containing the power to seal the Origin Tree, guarded by DragonBossIdle.", null, true, 1, "Dragon Seal Book", "Legendary", "None", "QuestItem" },
                    { 911, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A magic book containing the power to seal the Origin Tree, guarded by GolemBoss.", null, true, 1, "Golem Seal Book", "Legendary", "None", "QuestItem" },
                    { 912, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The final magic book to seal the Origin Tree, guarded by UnderKing.", null, true, 1, "UnderKing Seal Book", "Legendary", "None", "QuestItem" }
                });

            migrationBuilder.InsertData(
                table: "Quests",
                columns: new[] { "QuestId", "BossMonsterId", "DefaultStatus", "Description", "IsActive", "MapName", "ObjectiveLocation", "ObjectiveTarget", "ObjectiveType", "QuestGiverName", "RegionName", "RequiredLevel", "RewardExperience", "RewardGems", "RewardGold", "RewardItemId", "RewardSkillId", "TargetAmount", "Title", "Type" },
                values: new object[,]
                {
                    { 1, 2, "NotStarted", "Defeat the Swamp Demon to retrieve the Swamp Seal Book.", true, "Swamp", null, null, "Defeat", null, null, 1, 0, 0m, 0m, null, null, 1, "Defeat the Swamp Demon", "Main" },
                    { 2, 7, "NotStarted", "Defeat the Dragon Boss to retrieve the Dragon Seal Book.", true, "Volcano", null, null, "Defeat", null, null, 1, 0, 0m, 0m, null, null, 1, "Slay the Dragon Boss", "Main" },
                    { 3, 10, "NotStarted", "Defeat the Golem Boss to retrieve the Golem Seal Book.", true, "Desert", null, null, "Defeat", null, null, 1, 0, 0m, 0m, null, null, 1, "Crush the Golem Boss", "Main" },
                    { 4, 15, "NotStarted", "Defeat the UnderKing to retrieve the final Seal Book.", true, "Underworld", null, null, "Defeat", null, null, 1, 0, 0m, 0m, null, null, 1, "Vanquish the UnderKing", "Main" },
                    { 5, null, "NotStarted", "Collect the 4 Seal Books to purify the cursed Origin Tree.", true, "ElfForest", null, null, "Collect", null, null, 1, 0, 0m, 0m, null, null, 4, "Purify the Origin Tree", "Main" }
                });

            migrationBuilder.InsertData(
                table: "MonsterDrops",
                columns: new[] { "MonsterDropId", "DropRate", "IsActive", "IsGuaranteed", "ItemId", "MaxQuantity", "MinQuantity", "MonsterId" },
                values: new object[,]
                {
                    { 909, 100.0, true, true, 909, 1, 1, 2 },
                    { 910, 100.0, true, true, 910, 1, 1, 7 },
                    { 911, 100.0, true, true, 911, 1, 1, 10 },
                    { 912, 100.0, true, true, 912, 1, 1, 15 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MonsterDrops",
                keyColumn: "MonsterDropId",
                keyValue: 909);

            migrationBuilder.DeleteData(
                table: "MonsterDrops",
                keyColumn: "MonsterDropId",
                keyValue: 910);

            migrationBuilder.DeleteData(
                table: "MonsterDrops",
                keyColumn: "MonsterDropId",
                keyValue: 911);

            migrationBuilder.DeleteData(
                table: "MonsterDrops",
                keyColumn: "MonsterDropId",
                keyValue: 912);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 909);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 910);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 911);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 912);
        }
    }
}
