namespace DAL.Models
{
    public class MapMonster
    {
        public Guid Id { get; set; }

        public Guid GameMapId { get; set; }
        public GameMap? GameMap { get; set; }

        public Guid MonsterId { get; set; }
        public Monster? Monster { get; set; }

        public int SpawnWeight { get; set; } = 1;
        public int MinLevel { get; set; } = 1;
        public int MaxLevel { get; set; } = 10;

        public bool IsActive { get; set; } = true;
    }
}
