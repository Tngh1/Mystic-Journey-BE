using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the EquipmentStatsResponseDto class.
    public class EquipmentStatsResponseDto
    {
        // Executes equipment stats id operation.
        public int EquipmentStatsId { get; set; }
        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item name operation.
        public string? ItemName { get; set; }
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
        public float BonusMoveSpeed { get; set; }
        // Executes bonus attack speed operation.
        public float BonusAttackSpeed { get; set; }
        // Executes bonus crit rate operation.
        public float BonusCritRate { get; set; }
        // Executes bonus crit damage operation.
        public float BonusCritDamage { get; set; }
        // Executes bonus damage bonus operation.
        public float BonusDamageBonus { get; set; }
        // Executes total hp operation.
        public int TotalHp => BaseHp + BonusHp;
        // Executes total atk operation.
        public int TotalAtk => BaseAtk + BonusAtk;
        // Executes total def operation.
        public int TotalDef => BaseDef + BonusDef;
    }

    // Executes create equipment stats request dto operation.
    public class CreateEquipmentStatsRequestDto
    {
        // Executes item id operation.
        [Required]
        public int ItemId { get; set; }

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

    // Executes update equipment stats request dto operation.
    public class UpdateEquipmentStatsRequestDto
    {
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
        // Executes bonus physical power operation.
        public int BonusPhysicalPower { get; set; }
        // Executes bonus magic power operation.
        public int BonusMagicPower { get; set; }
    }
}
