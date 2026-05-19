namespace DAL.Models
{
    public class ChestItem
    {
        public Guid Id { get; set; }

        public Guid ChestId { get; set; }
        public Chest? Chest { get; set; }

        public Guid ItemId { get; set; }
        public Item? Item { get; set; }

        public int QuantityMin { get; set; } = 1;
        public int QuantityMax { get; set; } = 1;

        public decimal DropRate { get; set; } = 0;
        public bool IsGuaranteed { get; set; } = false;
    }
}
