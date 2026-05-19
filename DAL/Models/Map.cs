using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class GameMap
    {
        public Guid Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Types: Plains, Forest, Desert, Mountain, Cave, Dungeon, Town
        public string Type { get; set; } = "Plains";

        public int LevelRequirement { get; set; } = 1;
        public int MaxPlayers { get; set; } = 1;

        public decimal GoldReward { get; set; } = 0;
        public int ExperienceReward { get; set; } = 0;

        public bool IsUnlocked { get; set; } = true;

        public ICollection<MapMonster> MapMonsters { get; set; } = new List<MapMonster>();
    }
}
