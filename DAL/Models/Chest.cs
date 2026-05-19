using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Chest
    {
        public Guid Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Types: Common, Rare, Epic, Legendary
        public string Type { get; set; } = "Common";

        public int GoldMinReward { get; set; } = 0;
        public int GoldMaxReward { get; set; } = 0;
        public int ExperienceReward { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public ICollection<ChestItem> ChestItems { get; set; } = new List<ChestItem>();
    }
}
