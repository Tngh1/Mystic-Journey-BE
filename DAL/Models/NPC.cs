using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class NPC
    {
        public int NPCId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Types: Shopkeeper, QuestGiver, Trainer, Blacksmith, Healer, Merchant, Information
        public string Type { get; set; } = "Information";

        [MaxLength(100)]
        public string MapName { get; set; } = "ElfForest";

        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public float InteractionRadius { get; set; } = 2.5f;

        public string? IconUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<NPCDialogue> Dialogues { get; set; } = new List<NPCDialogue>();
    }
}
