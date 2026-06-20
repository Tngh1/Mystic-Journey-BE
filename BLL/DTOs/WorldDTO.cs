using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class PlayerWorldPositionDto
    {
        public string MapName { get; set; } = "ElfForest";
        public double PositionX { get; set; }
        public double PositionY { get; set; }
    }

    public class WorldMapProgressDto
    {
        public string MapName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsUnlocked { get; set; }
        public int ExplorationPercent { get; set; }
    }

    public class WorldStateResponseDto
    {
        public int PlayerProfileId { get; set; }
        public PlayerWorldPositionDto Position { get; set; } = new();
        public List<WorldMapProgressDto> Maps { get; set; } = new();
        public List<NPCResponseDto> Npcs { get; set; } = new();
        public List<PlayerQuestResponseDto> Quests { get; set; } = new();
        public PlayerQuestResponseDto? ActiveQuest { get; set; }
        public PlayerDailyLoginResponseDto? DailyLogin { get; set; }
    }

    public class UpdateWorldPositionRequestDto
    {
        [Required]
        [StringLength(100)]
        public string MapName { get; set; } = "ElfForest";

        public double PositionX { get; set; }
        public double PositionY { get; set; }
    }

    public class TalkToNpcRequestDto
    {
        [Required]
        public int NPCId { get; set; }
    }

    public class TalkToNpcResponseDto
    {
        public NPCResponseDto Npc { get; set; } = new();
        public List<PlayerQuestResponseDto> LinkedQuests { get; set; } = new();
    }

    public class InteractObjectRequestDto
    {
        [Required]
        [StringLength(100)]
        public string MapName { get; set; } = "ElfForest";

        [Required]
        [StringLength(150)]
        public string ObjectKey { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string InteractionType { get; set; } = "Interact";

        public int? QuestId { get; set; }
        public int ProgressDelta { get; set; } = 1;
    }

    public class InteractObjectResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public PlayerQuestResponseDto? Quest { get; set; }
        public int? CollectedItemId { get; set; }
        public string? CollectedItemName { get; set; }
        public int CollectedQuantity { get; set; }
    }

    public class TurnInQuestItemRequestDto
    {
        [Required]
        public int NPCId { get; set; }

        [Required]
        public int QuestId { get; set; }
    }

    public class TurnInQuestItemResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public PlayerQuestResponseDto? Quest { get; set; }
        public int? ConsumedItemId { get; set; }
        public string? ConsumedItemName { get; set; }
        public int ConsumedQuantity { get; set; }
    }
    public class OpenWorldChestRequestDto
    {
        public int? ChestId { get; set; }
        public int? PlayerChestId { get; set; }
    }
}
