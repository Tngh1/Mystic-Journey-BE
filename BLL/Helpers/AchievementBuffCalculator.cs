using System.Globalization;
using System.Text.RegularExpressions;

namespace BLL.Helpers
{
    /// <summary>
    /// Parses achievement BuffDescription strings (e.g. "+2% Max HP") into additive percentage bonuses.
    /// </summary>
    public sealed class AchievementBuffTotals
    {
        public decimal MaxHpPercent { get; set; }
        public decimal AtkPercent { get; set; }
        public decimal DefPercent { get; set; }
        public decimal MoveSpeedPercent { get; set; }
        public decimal CritRatePercent { get; set; }
        public decimal AttackSpeedPercent { get; set; }
        public decimal DamageBonusPercent { get; set; }
        public decimal GoldGainPercent { get; set; }
        public decimal ExpGainPercent { get; set; }
        public decimal BossDamagePercent { get; set; }
    }

    public static class AchievementBuffCalculator
    {
        private static readonly Regex SegmentRegex = new(
            @"\+(\d+(?:\.\d+)?)\s*%\s*(.+?)(?=,\s*\+|\s*$)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static AchievementBuffTotals ParseMany(IEnumerable<string?> descriptions)
        {
            var totals = new AchievementBuffTotals();
            foreach (var description in descriptions)
            {
                if (string.IsNullOrWhiteSpace(description))
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

        public static int ApplyPercent(int baseValue, decimal percentBonus)
        {
            if (baseValue <= 0 || percentBonus <= 0)
                return baseValue;

            var scaled = baseValue * (1m + percentBonus / 100m);
            return (int)Math.Round(scaled, MidpointRounding.AwayFromZero);
        }

        public static float ApplyPercent(float baseValue, decimal percentBonus)
        {
            if (baseValue <= 0f || percentBonus <= 0)
                return baseValue;

            return (float)(baseValue * (double)(1m + percentBonus / 100m));
        }
    }
}
