using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the NPCResponseDto class.
    public class NPCResponseDto
    {
        // Executes npc id operation.
        public int NPCId { get; set; }
        // Executes name operation.
        public string Name { get; set; } = string.Empty;
        // Executes description operation.
        public string? Description { get; set; }
        // NPC type is a free-form category with Information as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "Information";
        // Executes map name operation.
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
        public bool IsActive { get; set; }
        // Executes dialogues operation.
        public List<NPCDialogueResponseDto> Dialogues { get; set; } = new();
    }

    // Executes create npc request dto operation.
    public class CreateNPCRequestDto
    {
        // Executes name operation.
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        [StringLength(500)]
        public string? Description { get; set; }

        // NPC type is a free-form category with Information as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "Information";
        // Executes map name operation.
        [StringLength(100)]
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
    }

    // Executes update npc request dto operation.
    public class UpdateNPCRequestDto
    {
        // Executes name operation.
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        [StringLength(500)]
        public string? Description { get; set; }

        // NPC type is a free-form category with Information as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "Information";
        // Executes map name operation.
        [StringLength(100)]
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
    }

    // Executes npc dialogue response dto operation.
    public class NPCDialogueResponseDto
    {
        // Executes npc dialogue id operation.
        public int NPCDialogueId { get; set; }
        // Executes npc id operation.
        public int NPCId { get; set; }
        // Executes npc name operation.
        public string? NPCName { get; set; }
        // Executes content operation.
        public string Content { get; set; } = string.Empty;
        // Executes response type operation.
        public string ResponseType { get; set; } = "None";
        // Executes linked quest id operation.
        public int? LinkedQuestId { get; set; }
        // Executes linked quest title operation.
        public string? LinkedQuestTitle { get; set; }
        // Executes linked shop item id operation.
        public int? LinkedShopItemId { get; set; }
        // Executes linked shop item name operation.
        public string? LinkedShopItemName { get; set; }
        // Executes display order operation.
        public int DisplayOrder { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
    }

    // Executes create npc dialogue request dto operation.
    public class CreateNPCDialogueRequestDto
    {
        // Executes npc id operation.
        [Required]
        public int NPCId { get; set; }

        // Executes content operation.
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; } = string.Empty;

        // Executes response type operation.
        public string ResponseType { get; set; } = "None";
        // Executes linked quest id operation.
        public int? LinkedQuestId { get; set; }
        // Executes linked shop item id operation.
        public int? LinkedShopItemId { get; set; }
        // Executes display order operation.
        public int DisplayOrder { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes update npc dialogue request dto operation.
    public class UpdateNPCDialogueRequestDto
    {
        // Executes content operation.
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; } = string.Empty;

        // Executes response type operation.
        public string ResponseType { get; set; } = "None";
        // Executes linked quest id operation.
        public int? LinkedQuestId { get; set; }
        // Executes linked shop item id operation.
        public int? LinkedShopItemId { get; set; }
        // Executes display order operation.
        public int DisplayOrder { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }
}
