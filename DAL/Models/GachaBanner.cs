using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class GachaBanner
    {
        public int GachaBannerId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Types: Weapon, Character, Standard, Limited
        public string Type { get; set; } = "Weapon";

        public int PullCost { get; set; } = 100;
        public int PityLimit { get; set; } = 90;

        public bool IsActive { get; set; } = true;

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public ICollection<GachaBannerItem> BannerItems { get; set; } = new List<GachaBannerItem>();
        public ICollection<GachaPullHistory> PullHistories { get; set; } = new List<GachaPullHistory>();
    }
}