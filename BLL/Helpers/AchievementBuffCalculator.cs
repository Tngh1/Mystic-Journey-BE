using System.Globalization;
using System.Text.RegularExpressions;

namespace BLL.Helpers
{
    // Initializes a new default instance of the AchievementBuffTotals class.
    public sealed class AchievementBuffTotals
    {
        // Executes max hp percent operation.
        public decimal MaxHpPercent { get; set; }
        // Executes atk percent operation.
        public decimal AtkPercent { get; set; }
        // Executes def percent operation.
        public decimal DefPercent { get; set; }
        // Executes move speed percent operation.
        public decimal MoveSpeedPercent { get; set; }
        // Executes crit rate percent operation.
        public decimal CritRatePercent { get; set; }
        // Executes attack speed percent operation.
        public decimal AttackSpeedPercent { get; set; }
        // Executes damage bonus percent operation.
        public decimal DamageBonusPercent { get; set; }
        // Executes gold gain percent operation.
        public decimal GoldGainPercent { get; set; }
        // Executes exp gain percent operation.
        public decimal ExpGainPercent { get; set; }
        // Executes boss damage percent operation.
        // Validates input parameters against null or empty values.
        public decimal BossDamagePercent { get; set; }
    }

    // Executes achievement buff calculator operation.
    // Validates input parameters against null or empty values.
    public static class AchievementBuffCalculator
    {
        private static readonly Regex SegmentRegex = new(
            @"\+(\d+(?:\.\d+)?)\s*%\s*(.+?)(?=,\s*\+|\s*$)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Executes parse many operation.
        // Validates input parameters against null or empty values.
        public static AchievementBuffTotals ParseMany(IEnumerable<string?> descriptions)
        {
            var totals = new AchievementBuffTotals();
            foreach (var description in descriptions)
            {
                if (string.IsNullOrWhiteSpace(description))  // Mandatory string argument is blank — fail fast
                    continue;

                foreach (Match match in SegmentRegex.Matches(description))
                {
                    if (!decimal.TryParse(match.Groups[1].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var percent))
                        continue;

                    ApplySegment(totals, match.Groups[2].Value.Trim(), percent);
                }
            }

            return totals;
        }

        // Executes apply segment operation.
        private static void ApplySegment(AchievementBuffTotals totals, string label, decimal percent)
        {
            var normalized = label.ToLowerInvariant();

            if (normalized.Contains("all stats") || normalized.Contains("to all stats"))
            {
                totals.MaxHpPercent += percent;
                totals.AtkPercent += percent;
                totals.DefPercent += percent;
                return;
            }

            if (normalized.Contains("max hp") || normalized == "hp")
            {
                totals.MaxHpPercent += percent;
                return;
            }

            if (normalized.Contains("attack") || normalized == "atk")
            {
                totals.AtkPercent += percent;
                return;
            }

            if (normalized.Contains("defense") || normalized == "def")
            {
                totals.DefPercent += percent;
                return;
            }

            if (normalized.Contains("movement speed") || normalized.Contains("move speed"))
            {
                totals.MoveSpeedPercent += percent;
                return;
            }

            if (normalized.Contains("critical rate") || normalized.Contains("crit rate"))
            {
                totals.CritRatePercent += percent;
                return;
            }

            if (normalized.Contains("attack speed"))
            {
                totals.AttackSpeedPercent += percent;
                return;
            }

            if (normalized.Contains("gold gain") || normalized.Contains("gold"))
            {
                totals.GoldGainPercent += percent;
                return;
            }

            if (normalized.Contains("exp gain") || normalized.Contains("exp"))
            {
                totals.ExpGainPercent += percent;
                return;
            }

            if (normalized.Contains("boss"))
            {
                totals.BossDamagePercent += percent;
                return;
            }

            if (normalized.Contains("damage"))
            {
                totals.DamageBonusPercent += percent;
            }
        }

        // Executes apply percent operation.
        public static int ApplyPercent(int baseValue, decimal percentBonus)
        {
            if (baseValue <= 0 || percentBonus <= 0)
                return baseValue;

            var scaled = baseValue * (1m + percentBonus / 100m);
            return (int)Math.Round(scaled, MidpointRounding.AwayFromZero);
        }

        // Executes apply percent operation.
        public static float ApplyPercent(float baseValue, decimal percentBonus)
        {
            if (baseValue <= 0f || percentBonus <= 0)
                return baseValue;

            return (float)(baseValue * (double)(1m + percentBonus / 100m));
        }

        // Executes combine max hp operation.
        public static int CombineMaxHp(int baseMaxHp, int gearMaxHp, decimal achievementPercent)
        {
            return Math.Max(0, ApplyPercent(baseMaxHp + gearMaxHp, achievementPercent));
        }
    }
}
