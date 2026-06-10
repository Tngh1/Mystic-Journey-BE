using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ EquipmentStats ============
    public class EquipmentStatsResponseDto
    {
        public int EquipmentStatsId { get; set; }
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public int BaseHp { get; set; }
        public int BaseAtk { get; set; }
        public int BaseDef { get; set; }
        public int BonusHp { get; set; }
        public int BonusAtk { get; set; }
        public int BonusDef { get; set; }
        public float BonusMoveSpeed { get; set; }
        public float BonusAttackSpeed { get; set; }
        public float BonusCritRate { get; set; }
        public float BonusCritDamage { get; set; }
        public float BonusDamageBonus { get; set; }

        public int TotalHp => BaseHp + BonusHp;
        public int TotalAtk => BaseAtk + BonusAtk;
        public int TotalDef => BaseDef + BonusDef;
    }

    public class CreateEquipmentStatsRequestDto
    {
        [Required]
        public int ItemId { get; set; }

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

    public class UpdateEquipmentStatsRequestDto
    {
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
