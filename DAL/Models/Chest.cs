using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the Chest class.
    public class Chest
    {
        // Executes chest id operation.
        public int ChestId { get; set; }

        // Executes name operation.
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        public string? Description { get; set; }

        // Chest type is a free-form category with Common as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "Common";

        // Executes gold min reward operation.
        public int GoldMinReward { get; set; } = 0;
        // Executes gold max reward operation.
        public int GoldMaxReward { get; set; } = 0;
        // Executes experience reward operation.
        public int ExperienceReward { get; set; } = 0;

        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes chest items operation.
        public ICollection<ChestItem> ChestItems { get; set; } = new List<ChestItem>();
    }
}
