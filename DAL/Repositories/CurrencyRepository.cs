using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Results;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DAL.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly MysticJourneyDbContext _context;

        public CurrencyRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerProfile?> GetPlayerProfile(int playerProfileId)
        {
            return await _context.PlayerProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId);
        }

        public async Task<CurrencySpendResult> TrySpendCurrency(
            int playerProfileId,
            string currency,
            decimal amount,
            string reason,
            DateTime utcNow)
        {
            if (amount <= 0)
            {
                return new CurrencySpendResult { Status = CurrencySpendStatus.InvalidAmount };
            }

            var normalizedCurrency = NormalizeCurrency(currency);
            if (normalizedCurrency == null)
            {
                return new CurrencySpendResult { Status = CurrencySpendStatus.UnsupportedCurrency };
            }

            await using var dbTransaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var profile = await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId);

            if (profile == null)
            {
                return new CurrencySpendResult { Status = CurrencySpendStatus.PlayerNotFound };
            }

            var balanceBefore = GetBalance(profile, normalizedCurrency);
            if (balanceBefore < amount)
            {
                return new CurrencySpendResult
                {
                    Status = CurrencySpendStatus.InsufficientCurrency,
                    PlayerProfile = profile,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = balanceBefore
                };
            }

            var balanceAfter = balanceBefore - amount;
            SetBalance(profile, normalizedCurrency, balanceAfter);

            var log = new PlayerCurrencyLog
            {
                PlayerProfileId = playerProfileId,
                Currency = normalizedCurrency,
                Type = "Spend",
                Amount = -amount,
                BalanceAfter = balanceAfter,
                Note = reason,
                CreatedAt = utcNow
            };

            await _context.PlayerCurrencyLogs.AddAsync(log);
            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return new CurrencySpendResult
            {
                Status = CurrencySpendStatus.Success,
                PlayerProfile = profile,
                CurrencyLog = log,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter
            };
        }

        private static string? NormalizeCurrency(string? currency)
        {
            if (string.Equals(currency, "Gold", StringComparison.OrdinalIgnoreCase))
                return "Gold";

            if (string.Equals(currency, "Gems", StringComparison.OrdinalIgnoreCase))
                return "Gems";

            return null;
        }

        private static decimal GetBalance(PlayerProfile profile, string currency)
        {
            return currency == "Gold" ? profile.Gold : profile.Gems;
        }

        private static void SetBalance(PlayerProfile profile, string currency, decimal value)
        {
            if (currency == "Gold")
            {
                profile.Gold = value;
                return;
            }

            profile.Gems = value;
        }
    }
}
