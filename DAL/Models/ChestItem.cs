namespace DAL.Models
{
    // Initializes a new default instance of the ChestItem class.
    public class ChestItem
    {
        // Executes chest item id operation.
        public int ChestItemId { get; set; }

        // Executes chest id operation.
        public int ChestId { get; set; }
        // Executes chest operation.
        public Chest? Chest { get; set; }

        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item operation.
        public Item? Item { get; set; }

        // Executes quantity min operation.
        public int QuantityMin { get; set; } = 1;
        // Executes quantity max operation.
        public int QuantityMax { get; set; } = 1;

        // Executes drop rate operation.
        public decimal DropRate { get; set; } = 0;
        // Executes is guaranteed operation.
        public bool IsGuaranteed { get; set; } = false;
    }
}
