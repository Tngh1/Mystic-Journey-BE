using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the DungeonConfigResponseDto class.
    public class DungeonConfigResponseDto
    {
        // Executes dungeon config id operation.
        public int DungeonConfigId { get; set; }
        // Executes name operation.
        public string Name { get; set; } = string.Empty;
        // Executes description operation.
        public string? Description { get; set; }
        // Dungeon type is a free-form category with Normal as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "Normal";
        // Executes level requirement operation.
        public int LevelRequirement { get; set; }
        // Executes max members operation.
        public int MaxMembers { get; set; }
        // Executes difficulty operation.
        public int Difficulty { get; set; }
        // Executes recommended power operation.
        public int RecommendedPower { get; set; }
        // Executes energy cost operation.
        public int EnergyCost { get; set; }
        // Executes chest id operation.
        public int? ChestId { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
        // Executes gold min reward operation.
        public int GoldMinReward { get; set; }
        // Executes gold max reward operation.
        public int GoldMaxReward { get; set; }
        // Executes experience reward operation.
        public int ExperienceReward { get; set; }
        // Executes possible drops operation.
        public List<ChestItemResponseDto> PossibleDrops { get; set; } = new();
    }

    // Executes update dungeon config request dto operation.
    public class UpdateDungeonConfigRequestDto
    {
        // Executes name operation.
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;
        // Executes description operation.
        public string? Description { get; set; }
        // Dungeon type is a free-form category with Normal as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "Normal";
        // Executes level requirement operation.
        public int LevelRequirement { get; set; } = 1;
        // Executes max members operation.
        public int MaxMembers { get; set; } = 4;
        // Executes difficulty operation.
        public int Difficulty { get; set; } = 1;
        // Executes recommended power operation.
        public int RecommendedPower { get; set; } = 0;
        // Executes energy cost operation.
        public int EnergyCost { get; set; } = 10;
        // Executes chest id operation.
        public int? ChestId { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }


    // Executes enter dungeon request dto operation.
    public class EnterDungeonRequestDto
    {
        // Executes party members operation.
        public List<string>? PartyMembers { get; set; }
    }

    // Executes enter dungeon response dto operation.
    public class EnterDungeonResponseDto
    {
        // Executes dungeon session id operation.
        public int DungeonSessionId { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes dungeon config id operation.
        public int DungeonConfigId { get; set; }
        // Executes dungeon name operation.
        public string DungeonName { get; set; } = string.Empty;
        // Executes energy cost operation.
        public int EnergyCost { get; set; }
        // Executes player current energy operation.
        public int PlayerCurrentEnergy { get; set; }
        // Executes enter time operation.
        public DateTime EnterTime { get; set; }
        // Supported dungeon session states: Active, Completed, Abandoned, Failed, Expired, or RewardClaimed; transitions control progress and reward eligibility.
        public string Status { get; set; } = "Active";
        // Executes party members operation.
        public List<string> PartyMembers { get; set; } = new();

        // Executes progress operation.
        public DungeonProgressResponseDto? Progress { get; set; }
    }


    // Executes update dungeon progress request dto operation.
    public class UpdateDungeonProgressRequestDto
    {
        // Executes monsters killed operation.
        [Range(0, int.MaxValue, ErrorMessage = "MonstersKilled cannot be negative.")]
        public int MonstersKilled { get; set; } = 0;

        // Executes boss killed operation.
        public bool BossKilled { get; set; } = false;

        // Executes completion percentage operation.
        [Range(0, 100, ErrorMessage = "CompletionPercentage must be between 0 and 100.")]
        public int CompletionPercentage { get; set; } = 0;

        // Executes extra data operation.
        public string? ExtraData { get; set; }

        // Executes boss spawned operation.
        public bool BossSpawned { get; set; } = false;
        // Executes elapsed time operation.
        public int ElapsedTime { get; set; } = 0;
    }

    // Executes dungeon progress response dto operation.
    public class DungeonProgressResponseDto
    {
        // Executes dungeon progress id operation.
        public int DungeonProgressId { get; set; }
        // Executes dungeon session id operation.
        public int DungeonSessionId { get; set; }
        // Executes monsters killed operation.
        public int MonstersKilled { get; set; }
        // Executes boss killed operation.
        public bool BossKilled { get; set; }
        // Executes completion percentage operation.
        public int CompletionPercentage { get; set; }
        // Executes extra data operation.
        public string? ExtraData { get; set; }
        // Executes boss spawned operation.
        public bool BossSpawned { get; set; }
        // Executes elapsed time operation.
        public int ElapsedTime { get; set; }
        // Executes updated at operation.
        public DateTime? UpdatedAt { get; set; }
        // Supported dungeon session states: Active, Completed, Abandoned, Failed, Expired, or RewardClaimed; transitions control progress and reward eligibility.
        public string SessionStatus { get; set; } = string.Empty;
    }


    // Executes dungeon history response dto operation.
    public class DungeonHistoryResponseDto
    {
        // Executes dungeon session id operation.
        public int DungeonSessionId { get; set; }
        // Executes dungeon name operation.
        public string DungeonName { get; set; } = string.Empty;
        // Executes difficulty operation.
        public int Difficulty { get; set; }
        // Supported dungeon session states: Active, Completed, Abandoned, Failed, Expired, or RewardClaimed; transitions control progress and reward eligibility.
        public string Status { get; set; } = string.Empty;
        // Executes elapsed time operation.
        public int ElapsedTime { get; set; }
        // Executes completion percentage operation.
        public int CompletionPercentage { get; set; }
        // Executes enter time operation.
        public DateTime EnterTime { get; set; }
        // Executes completed time operation.
        public DateTime? CompletedTime { get; set; }
    }


    // Executes complete dungeon response dto operation.
    public class CompleteDungeonResponseDto
    {
        // Executes dungeon session id operation.
        public int DungeonSessionId { get; set; }
        // Supported dungeon session states: Active, Completed, Abandoned, Failed, Expired, or RewardClaimed; transitions control progress and reward eligibility.
        public string Status { get; set; } = "Completed";
        // Executes completed time operation.
        public DateTime CompletedTime { get; set; }
        // Executes reward chest operation.
        public ChestResponseDto? RewardChest { get; set; }
        // Executes message operation.
        public string Message { get; set; } = string.Empty;
    }


    // Executes claim dungeon reward response dto operation.
    public class ClaimDungeonRewardResponseDto
    {
        // Executes dungeon session id operation.
        public int DungeonSessionId { get; set; }
        // Executes success operation.
        public bool Success { get; set; }
        // Executes message operation.
        public string Message { get; set; } = string.Empty;
        // Executes energy consumed operation.
        public int EnergyConsumed { get; set; }
        // Executes gold earned operation.
        public int GoldEarned { get; set; }
        // Executes experience earned operation.
        public int ExperienceEarned { get; set; }
        // Executes time taken seconds operation.
        public float TimeTakenSeconds { get; set; }
        // Executes items operation.
        public List<DungeonRewardItemDto> Items { get; set; } = new();

        // Executes wallet operation.
        public WalletDto? Wallet { get; set; }
        // Executes character operation.
        public CharacterDto? Character { get; set; }
    }

    // Executes wallet dto operation.
    public class WalletDto
    {
        // Executes gold operation.
        public decimal Gold { get; set; }
        // Executes gems operation.
        public decimal Gems { get; set; }
    }

    // Executes character dto operation.
    public class CharacterDto
    {
        // Executes level operation.
        public int Level { get; set; }
        // Executes experience points operation.
        public int ExperiencePoints { get; set; }
        // Executes energy operation.
        public int Energy { get; set; }
        // Executes max energy operation.
        public int MaxEnergy { get; set; }
    }

    // Executes dungeon reward item dto operation.
    public class DungeonRewardItemDto
    {
        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item name operation.
        public string ItemName { get; set; } = string.Empty;
        // Executes item icon url operation.
        public string? ItemIconUrl { get; set; }
        // Executes item type operation.
        public string ItemType { get; set; } = string.Empty;
        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string Rarity { get; set; } = string.Empty;
        // Executes quantity operation.
        public int Quantity { get; set; }
    }
}
