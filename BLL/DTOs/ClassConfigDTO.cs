namespace BLL.DTOs
{
    public class ClassConfigResponseDto
    {
        public int ClassConfigId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int MaxHp { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int MoveSpeed { get; set; }
        public int AttackSpeed { get; set; }
        public int CritRate { get; set; }
        public int CritDamage { get; set; }
        public int DamageBonus { get; set; }
    }
}
