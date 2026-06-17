using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ NPC ============
    public class NPCResponseDto
    {
        public int NPCId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = "Information";
        public string MapName { get; set; } = "ElfForest";
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public float InteractionRadius { get; set; } = 2.5f;
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; }
        public List<NPCDialogueResponseDto> Dialogues { get; set; } = new();
    }

    public class CreateNPCRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string Type { get; set; } = "Information";
        [StringLength(100)]
        public string MapName { get; set; } = "ElfForest";
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public float InteractionRadius { get; set; } = 2.5f;
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateNPCRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string Type { get; set; } = "Information";
        [StringLength(100)]
        public string MapName { get; set; } = "ElfForest";
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public float InteractionRadius { get; set; } = 2.5f;
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ============ NPCDialogue ============
    public class NPCDialogueResponseDto
    {
        public int NPCDialogueId { get; set; }
        public int NPCId { get; set; }
        public string? NPCName { get; set; }
        public string Content { get; set; } = string.Empty;
        public string ResponseType { get; set; } = "None";
        public int? LinkedQuestId { get; set; }
        public string? LinkedQuestTitle { get; set; }
        public int? LinkedShopItemId { get; set; }
        public string? LinkedShopItemName { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateNPCDialogueRequestDto
    {
        [Required]
        public int NPCId { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; } = string.Empty;

        public string ResponseType { get; set; } = "None";
        public int? LinkedQuestId { get; set; }
        public int? LinkedShopItemId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateNPCDialogueRequestDto
    {
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; } = string.Empty;

        public string ResponseType { get; set; } = "None";
        public int? LinkedQuestId { get; set; }
        public int? LinkedShopItemId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
