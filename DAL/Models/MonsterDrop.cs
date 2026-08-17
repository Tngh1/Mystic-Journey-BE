using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the MonsterDrop class.
    public class MonsterDrop
    {
        // Executes monster drop id operation.
        public int MonsterDropId { get; set; }

        // Executes monster id operation.
        public int MonsterId { get; set; }
        // Executes monster operation.
        public Monster? Monster { get; set; }

        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item operation.
        public Item? Item { get; set; }

        // Executes drop rate operation.
        public double DropRate { get; set; }

        // Executes min quantity operation.
        public int MinQuantity { get; set; } = 1;

        // Executes max quantity operation.
        public int MaxQuantity { get; set; } = 1;

        // Executes is guaranteed operation.
        public bool IsGuaranteed { get; set; } = false;

        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }
}
