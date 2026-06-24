using System;
using System.Linq;
using System.Collections.Generic;

namespace DAL.Models
{
    public class PlayerDailyLogin
    {
        public int PlayerDailyLoginId { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int CurrentStreak { get; set; } = 0; // Có thể giữ lại để vinh danh, nhưng không còn dùng để track thưởng nữa
        public int TotalDaysClaimed { get; set; } = 0;
        public DateTime? LastClaimedAt { get; set; }
        public bool IsClaimedToday { get; set; } = false;

        // Calendar-based fields
        public int CurrentYear { get; set; } = DateTime.UtcNow.Year;
        public int CurrentMonth { get; set; } = DateTime.UtcNow.Month;
        
        // Track the number of retro-claims this month (limit: 5)
        public int RetroClaimCount { get; set; } = 0;

        // Comma-separated list of days claimed in the current month, e.g., "1,2,4,5"
        public string ClaimedDaysStr { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<int> ClaimedDays
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ClaimedDaysStr)) return new List<int>();
                return ClaimedDaysStr.Split(',').Select(int.Parse).ToList();
            }
            set
            {
                ClaimedDaysStr = value == null || !value.Any() ? string.Empty : string.Join(",", value);
            }
        }
    }
}
