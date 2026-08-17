namespace DAL.Models
{
    // Initializes a new default instance of the EquipmentStats class.
    public class EquipmentStats
    {
        // Executes equipment stats id operation.
        public int EquipmentStatsId { get; set; }

        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item operation.
        public Item? Item { get; set; }

        // Executes base hp operation.
        public int BaseHp { get; set; }
        // Executes base atk operation.
        public int BaseAtk { get; set; }
        // Executes base def operation.
        public int BaseDef { get; set; }

        // Executes bonus hp operation.
        public int BonusHp { get; set; }
        // Executes bonus atk operation.
        public int BonusAtk { get; set; }
        // Executes bonus def operation.
        public int BonusDef { get; set; }

        // Executes bonus move speed operation.
        public int BonusMoveSpeed { get; set; }
        // Executes bonus attack speed operation.
        public int BonusAttackSpeed { get; set; }

        // Executes bonus crit rate operation.
        public int BonusCritRate { get; set; }
        // Executes bonus crit damage operation.
        public int BonusCritDamage { get; set; }

        // Executes bonus damage bonus operation.
        public int BonusDamageBonus { get; set; }
    }
}
