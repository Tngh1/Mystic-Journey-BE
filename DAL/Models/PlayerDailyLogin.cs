namespace DAL.Models
{
    public class PlayerDailyLogin
    {
        public int PlayerDailyLoginId { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int CurrentStreak { get; set; } = 0;
        public int TotalDaysClaimed { get; set; } = 0;
        public DateTime? LastClaimedAt { get; set; }
        public bool IsClaimedToday { get; set; } = false;
    }
}
