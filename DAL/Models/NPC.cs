using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class NPC
    {
        public Guid Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Types: Shopkeeper, QuestGiver, Trainer, Blacksmith, Healer, Merchant, Information
        public string Type { get; set; } = "Information";

        public Guid? GameMapId { get; set; }
        public GameMap? GameMap { get; set; }

        public int PositionX { get; set; } = 0;
        public int PositionY { get; set; } = 0;

        public string? IconUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<NPCDialogue> Dialogues { get; set; } = new List<NPCDialogue>();
    }
}
