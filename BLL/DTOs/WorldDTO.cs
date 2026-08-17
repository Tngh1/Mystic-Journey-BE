using System.ComponentModel.DataAnnotations;
namespace BLL.DTOs
{
    // Initializes a new default instance of the PlayerWorldPositionDto class.
    public class PlayerWorldPositionDto
    {
        // Executes map name operation.
        public string MapName { get; set; } = "ElfForest";
        // Executes position x operation.
        public double PositionX { get; set; }
        // Executes position y operation.
        public double PositionY { get; set; }
    }

    // Executes world map progress dto operation.
    public class WorldMapProgressDto
    {
        // Executes map name operation.
        public string MapName { get; set; } = string.Empty;
        // Executes display name operation.
        public string DisplayName { get; set; } = string.Empty;
        // Executes is unlocked operation.
        public bool IsUnlocked { get; set; }
        // Executes exploration percent operation.
        public int ExplorationPercent { get; set; }
    }

    // Executes world state response dto operation.
    public class WorldStateResponseDto
    {
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes position operation.
        public PlayerWorldPositionDto Position { get; set; } = new();
        // Executes maps operation.
        public List<WorldMapProgressDto> Maps { get; set; } = new();
        // Executes npcs operation.
        public List<NPCResponseDto> Npcs { get; set; } = new();
        // Executes quests operation.
        public List<PlayerQuestResponseDto> Quests { get; set; } = new();
        // Executes active quest operation.
        public PlayerQuestResponseDto? ActiveQuest { get; set; }
        // Executes daily login operation.
        public PlayerDailyLoginResponseDto? DailyLogin { get; set; }
    }

    // Executes update world position request dto operation.
    public class UpdateWorldPositionRequestDto
    {
        // Executes map name operation.
        [Required]
        [StringLength(100)]
        public string MapName { get; set; } = "ElfForest";

        // Executes position x operation.
        public double PositionX { get; set; }
        // Executes position y operation.
        public double PositionY { get; set; }
    }

    // Executes talk to npc request dto operation.
    public class TalkToNpcRequestDto
    {
        // Executes npc id operation.
        [Required]
        public int NPCId { get; set; }
    }


    // Executes talk to npc response dto operation.
    public class TalkToNpcResponseDto
    {

        // Executes npc operation.
        public NPCResponseDto Npc { get; set; } = new();
        // Executes linked quests operation.
        public List<PlayerQuestResponseDto> LinkedQuests { get; set; } = new();
    }

    // Executes interact object request dto operation.
    public class InteractObjectRequestDto
    {
        // Executes map name operation.
        [Required]
        [StringLength(100)]
        public string MapName { get; set; } = "ElfForest";

        // Executes object key operation.
        [Required]
        [StringLength(150)]
        public string ObjectKey { get; set; } = string.Empty;

        // Executes interaction type operation.
        [Required]
        [StringLength(50)]
        public string InteractionType { get; set; } = "Interact";

        // Executes quest id operation.
        public int? QuestId { get; set; }

        // Executes progress delta operation.
        [Range(1, 100, ErrorMessage = "ProgressDelta must be between 1 and 100.")]
        public int ProgressDelta { get; set; } = 1;
    }

    // Executes interact object response dto operation.
    public class InteractObjectResponseDto
    {
        // Executes success operation.
        public bool Success { get; set; }
        // Executes message operation.
        public string Message { get; set; } = string.Empty;
        // Executes quest operation.
        public PlayerQuestResponseDto? Quest { get; set; }
        // Executes collected item id operation.
        public int? CollectedItemId { get; set; }
        // Executes collected item name operation.
        public string? CollectedItemName { get; set; }
        // Executes collected quantity operation.
        public int CollectedQuantity { get; set; }
    }

    // Executes turn in quest item request dto operation.
    public class TurnInQuestItemRequestDto
    {
        // Executes npc id operation.
        [Required]
        public int NPCId { get; set; }

        // Executes quest id operation.
        [Required]
        public int QuestId { get; set; }
    }

    // Executes turn in quest item response dto operation.
    public class TurnInQuestItemResponseDto
    {
        // Executes success operation.
        public bool Success { get; set; }
        // Executes message operation.
        public string Message { get; set; } = string.Empty;
        // Executes quest operation.
        public PlayerQuestResponseDto? Quest { get; set; }
        // Executes consumed item id operation.
        public int? ConsumedItemId { get; set; }
        // Executes consumed item name operation.
        public string? ConsumedItemName { get; set; }
        // Executes consumed quantity operation.
        public int ConsumedQuantity { get; set; }
    }

    // Executes open world chest request dto operation.
    public class OpenWorldChestRequestDto
    {
        // Executes chest id operation.
        public int? ChestId { get; set; }
        // Executes player chest id operation.
        public int? PlayerChestId { get; set; }
    }
}
