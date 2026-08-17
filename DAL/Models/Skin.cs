using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the Skin class.
    public class Skin
    {
        // Executes skin id operation.
        public int SkinId { get; set; }

        // Executes name operation.
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        public string? Description { get; set; }

        // Supported skin types include Armor and FullSet; the value identifies how the cosmetic is grouped and equipped.
        public string Type { get; set; } = "Armor";

        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string Rarity { get; set; } = "Common";

        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes preview url operation.
        public string? PreviewUrl { get; set; }

        // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
        public string Currency { get; set; } = "Gems";
        // Executes price operation.
        public decimal Price { get; set; } = 0;

        // Executes is for sale operation.
        public bool IsForSale { get; set; } = false;
        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Executes player skins operation.
        public ICollection<PlayerSkin> PlayerSkins { get; set; } = new List<PlayerSkin>();
    }
}
