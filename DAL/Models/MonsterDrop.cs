using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class MonsterDrop
    {
        public int MonsterDropId { get; set; }

        public int MonsterId { get; set; }
        public Monster? Monster { get; set; }

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public double DropRate { get; set; }

        public int MinQuantity { get; set; } = 1;

        public int MaxQuantity { get; set; } = 1;

        // Luôn rơi hay không
        public bool IsGuaranteed { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }
}