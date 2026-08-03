using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(DAL.Data.MysticJourneyDbContext))]
    [Migration("20260804000000_SeedSkins")]
    public partial class SeedSkins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ──────────────────────────────────────────────────────────────────
            // Seed 6 skins matching Unity SkinDatabase (Assets/Resources/SkinDatabase.asset)
            //
            // SkinId layout:
            //   1  – Knight Default   (default, Class = Knight, free)
            //   2  – Archer Default   (default, Class = Archer, free)
            //   3  – Mage Default     (default, Class = Mage,   free)
            //   4  – Knight Skin      (premium, Class = Knight, for sale)
            //   5  – Archer Skin      (premium, Class = Archer, for sale)
            //   6  – Mage Skin        (premium, Class = Mage,   for sale)
            // ──────────────────────────────────────────────────────────────────
            migrationBuilder.InsertData(
                table: "Skins",
                columns: new[]
                {
                    "SkinId", "Name", "Description", "Type", "Rarity",
                    "IconUrl", "PreviewUrl",
                    "Currency", "Price",
                    "IsForSale", "IsActive", "CreatedAt"
                },
                values: new object[,]
                {
                    // ── Default skins (free, not for sale) ──────────────────
                    {
                        1,
                        "Knight Default",
                        "The default armor worn by every Knight. Sturdy and battle-tested.",
                        "FullSet", "Common",
                        null, null,
                        "Gems", 0m,
                        false, true,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        2,
                        "Archer Default",
                        "The default outfit of every Archer. Light and built for speed.",
                        "FullSet", "Common",
                        null, null,
                        "Gems", 0m,
                        false, true,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        3,
                        "Mage Default",
                        "The default robes of every Mage. Woven with arcane thread.",
                        "FullSet", "Common",
                        null, null,
                        "Gems", 0m,
                        false, true,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                    },
                    // ── Premium skins (for sale) ─────────────────────────────
                    {
                        4,
                        "Archer Skin",
                        "Shadow-dyed leather with enchanted quiver – the mark of an elite forest ranger. Exclusive Archer cosmetic.",
                        "FullSet", "Epic",
                        null, null,
                        "Gems", 300m,
                        true, true,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        5,
                        "Knight Skin",
                        "A glorious silver set adorned with the crest of the Mystic Kingdom. Exclusive Knight cosmetic.",
                        "FullSet", "Epic",
                        null, null,
                        "Gems", 300m,
                        true, true,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                    },
                    {
                        6,
                        "Mage Skin",
                        "Starweave robes pulsing with celestial energy. Exclusive Mage cosmetic.",
                        "FullSet", "Epic",
                        null, null,
                        "Gems", 300m,
                        true, true,
                        new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                    },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Skins",
                keyColumn: "SkinId",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6 });
        }
    }
}
