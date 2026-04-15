namespace DAL.Models
{
    public class EquipmentStats
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }
        public Item? Item { get; set; }

        public int HealthBonus { get; set; } = 0;
        public int ManaBonus { get; set; } = 0;
        public int StrengthBonus { get; set; } = 0;
        public int DefenseBonus { get; set; } = 0;
        public int AgilityBonus { get; set; } = 0;
        public int IntelligenceBonus { get; set; } = 0;
        public int EnduranceBonus { get; set; } = 0;
        public int LuckBonus { get; set; } = 0;

        public int AttackBonus { get; set; } = 0;
        public int CriticalRateBonus { get; set; } = 0;
        public int CriticalDamageBonus { get; set; } = 0;
        public int ArmorPenetrationBonus { get; set; } = 0;
    }
}