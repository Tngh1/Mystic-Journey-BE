using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Skin
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Types: Armor, Weapon, Accessory, FullSet
        public string Type { get; set; } = "Armor";

        // Rarities: Common, Uncommon, Rare, Epic, Legendary, Mythic
        public string Rarity { get; set; } = "Common";

        public string? IconUrl { get; set; }
        public string? PreviewUrl { get; set; }

        // Currencies: Gold, Gems
        public string Currency { get; set; } = "Gems";
        public decimal Price { get; set; } = 0;

        public bool IsForSale { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PlayerSkin> PlayerSkins { get; set; } = new List<PlayerSkin>();
    }
}
