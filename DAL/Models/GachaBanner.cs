using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the GachaBanner class.
    public class GachaBanner
    {
        // Executes gacha banner id operation.
        public int GachaBannerId { get; set; }

        // Executes name operation.
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Supported gacha banner types: Standard, Limited, or Event; the type controls banner categorization and presentation.
        public string Type { get; set; } = "Weapon";

        // Executes pull cost operation.
        public int PullCost { get; set; } = 100;
        // Executes cost item id operation.
        public int? CostItemId { get; set; }
        // Executes pity limit operation.
        public int PityLimit { get; set; } = 90;

        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes start at operation.
        public DateTime StartAt { get; set; }
        // Executes end at operation.
        public DateTime EndAt { get; set; }

        // Executes banner items operation.
        public ICollection<GachaBannerItem> BannerItems { get; set; } = new List<GachaBannerItem>();
        // Executes pull histories operation.
        public ICollection<GachaPullHistory> PullHistories { get; set; } = new List<GachaPullHistory>();
    }
}
