namespace DAL.Models
{
    public class PlayerSkill
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public Guid SkillId { get; set; }
        public Skill? Skill { get; set; }

        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public bool IsEquipped { get; set; } = false;

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}