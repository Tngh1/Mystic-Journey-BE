namespace DAL.Models
{
    public class EquipmentStats
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public int BaseHp { get; set; }
        public int BaseAtk { get; set; }
        public int BaseDef { get; set; }

        public int BonusHp { get; set; }
        public int BonusAtk { get; set; }
        public int BonusDef { get; set; }

        public int BonusMoveSpeed { get; set; }
        public int BonusAttackSpeed { get; set; }

        public int BonusCritRate { get; set; }
        public int BonusCritDamage { get; set; }

        public int BonusDamageBonus { get; set; }
    }
}