namespace BLL.Utils
{
    public static class StatScale
    {
        // per-stat scales (store integers to avoid float precision issues)
        public const int CritRate = 10;      // 1 decimal (15.5% -> 155)
        public const int AttackSpeed = 100;  // 2 decimals (1.25 -> 125)
        public const int MoveSpeed = 100;    // 2 decimals
        public const int DamageBonus = 10;   // 1 decimal (20% -> 200? depends on definition)
    }
}
