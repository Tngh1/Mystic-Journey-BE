namespace DAL.Models
{
    public class PlayerSkill
    {
        public int PlayerSkillId { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int SkillId { get; set; }
        public Skill? Skill { get; set; }

        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;

        // EquippedSlot is the single source-of-truth for equip state (0..2).
        // Expose a derived convenience property for code that used IsEquipped.
        public int? EquippedSlot { get; set; } = null; // 0..2 for three slots, null = not equipped

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public bool IsEquipped => EquippedSlot.HasValue;

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
        public DateTime? NextAvailableTime { get; set; }
    }
}