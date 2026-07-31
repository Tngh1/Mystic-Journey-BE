using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMagicFlourToConsumable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 31,
                columns: new[] { "BaseValue", "CorruptionReduction", "Description", "Rarity", "Type" },
                values: new object[] { 50m, 0.5f, "Mystical flour imbued with purifying magic. Reduces your corruption by 50% when consumed.", "Uncommon", "Consumable" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 31,
                columns: new[] { "BaseValue", "CorruptionReduction", "Description", "Rarity", "Type" },
                values: new object[] { 0m, 0f, "Mystical flour used for special spells.", "Common", "QuestItem" });
        }
    }
}
