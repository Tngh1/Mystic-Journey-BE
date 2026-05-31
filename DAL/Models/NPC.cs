using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class NPC
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Types: Shopkeeper, QuestGiver, Trainer, Blacksmith, Healer, Merchant, Information
        public string Type { get; set; } = "Information";

        public string? IconUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<NPCDialogue> Dialogues { get; set; } = new List<NPCDialogue>();
    }
}
