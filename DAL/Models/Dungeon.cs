using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the Dungeon class.
    public class Dungeon
    {
        // Executes dungeon id operation.
        public int DungeonId { get; set; }

        // Executes name operation.
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        public string? Description { get; set; }

        // Executes is repeatable operation.
        public bool IsRepeatable { get; set; } = true;

        // Executes spawns operation.
        public ICollection<MonsterSpawn> Spawns { get; set; } = new List<MonsterSpawn>();
    }
}
