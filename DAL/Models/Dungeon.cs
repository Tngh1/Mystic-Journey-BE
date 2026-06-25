using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Dungeon
    {
        public int DungeonId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // If true, players can re-run this dungeon (spawns respawn normally).
        // If false, the dungeon bosses/monsters may be one-time or controlled by quest state.
        public bool IsRepeatable { get; set; } = true;

        public ICollection<MonsterSpawn> Spawns { get; set; } = new List<MonsterSpawn>();
    }
}
