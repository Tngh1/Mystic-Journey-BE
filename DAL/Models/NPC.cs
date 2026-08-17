using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the NPC class.
    public class NPC
    {
        // Executes npc id operation.
        public int NPCId { get; set; }

        // Executes name operation.
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        public string? Description { get; set; }

        // NPC type is a free-form category with Information as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "Information";

        // Executes map name operation.
        [MaxLength(100)]
        public string MapName { get; set; } = "ElfForest";

        // Executes position x operation.
        public double PositionX { get; set; }
        // Executes position y operation.
        public double PositionY { get; set; }
        // Executes interaction radius operation.
        public float InteractionRadius { get; set; } = 2.5f;

        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes dialogues operation.
        public ICollection<NPCDialogue> Dialogues { get; set; } = new List<NPCDialogue>();
    }
}
