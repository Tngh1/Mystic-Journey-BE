using System;
using System.Linq;
using System.Collections.Generic;

namespace DAL.Models
{
    // Initializes a new default instance of the PlayerDailyLogin class.
    public class PlayerDailyLogin
    {
        // Executes player daily login id operation.
        public int PlayerDailyLoginId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes current streak operation.
        public int CurrentStreak { get; set; } = 0;
        // Executes total days claimed operation.
        public int TotalDaysClaimed { get; set; } = 0;
        // Executes last claimed at operation.
        public DateTime? LastClaimedAt { get; set; }
        // Executes is claimed today operation.
        public bool IsClaimedToday { get; set; } = false;

        // Executes current year operation.
        public int CurrentYear { get; set; } = DateTime.UtcNow.Year;
        // Executes current month operation.
        public int CurrentMonth { get; set; } = DateTime.UtcNow.Month;

        // Executes retro claim count operation.
        public int RetroClaimCount { get; set; } = 0;

        // Executes claimed days str operation.
        // Validates input parameters against null or empty values.
        public string ClaimedDaysStr { get; set; } = string.Empty;

        // Executes claimed days operation.
        // Validates input parameters against null or empty values.
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<int> ClaimedDays
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ClaimedDaysStr)) return new List<int>();  // Mandatory string argument is blank — fail fast
                return ClaimedDaysStr.Split(',').Select(int.Parse).ToList();
            }
            set
            {
                ClaimedDaysStr = value == null || !value.Any() ? string.Empty : string.Join(",", value);
            }
        }
    }
}
