namespace DAL.Models
{
    // Initializes a new default instance of the PlayerSkill class.
    public class PlayerSkill
    {
        // Executes player skill id operation.
        public int PlayerSkillId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes skill id operation.
        public int SkillId { get; set; }
        // Executes skill operation.
        public Skill? Skill { get; set; }

        // Executes level operation.
        public int Level { get; set; } = 1;
        // Executes experience operation.
        public int Experience { get; set; } = 0;

        // Executes equipped slot operation.
        public int? EquippedSlot { get; set; } = null;

        // Executes is equipped operation.
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public bool IsEquipped => EquippedSlot.HasValue;

        // Executes unlocked at operation.
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
        // Executes next available time operation.
        public DateTime? NextAvailableTime { get; set; }
    }
}
