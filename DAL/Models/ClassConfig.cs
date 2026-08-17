using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the ClassConfig class.
    public class ClassConfig
    {
        // Executes class config id operation.
        public int ClassConfigId { get; set; }

        [Required]
        [MaxLength(50)]
        // Executes class name operation.
        public string ClassName { get; set; } = string.Empty;

        // Executes max hp operation.
        public int MaxHp { get; set; }
        // Executes atk operation.
        public int Atk { get; set; }
        // Executes def operation.
        public int Def { get; set; }
        // Executes move speed operation.
        public int MoveSpeed { get; set; }
        // Executes attack speed operation.
        public int AttackSpeed { get; set; }
        // Executes crit rate operation.
        public int CritRate { get; set; }
        // Executes crit damage operation.
        public int CritDamage { get; set; }
        // Executes damage bonus operation.
        public int DamageBonus { get; set; }
    }
}
