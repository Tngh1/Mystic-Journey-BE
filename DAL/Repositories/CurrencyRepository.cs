using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Results;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DAL.Repositories
{
    // Queries the database to retrieve i currency repository records.
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of CurrencyRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public CurrencyRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Performs database query and transactional persistence workflow for get player profile.
        // Query details: uses AsNoTracking() for read-only query optimization; executes within an atomic database transaction; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerProfile? entity result or default if not found.
        public async Task<PlayerProfile?> GetPlayerProfile(int playerProfileId)
        {
            return await _context.PlayerProfiles
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId);  // Fetch single matching record or null if not found
        }

        // Attempt spend currency using player profile id, currency, amount, and reason; it opens a database transaction, selects the matching record, loads balance, updates balance, and creates async and guards invalid or unavailable states and keeps dependent writes atomic.
        public async Task<CurrencySpendResult> TrySpendCurrency(
            int playerProfileId,
            // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
            string currency,
            decimal amount,
            string reason,
            DateTime utcNow)
        {
            if (amount <= 0)  // Reject non-positive heal/damage values
            {
                return new CurrencySpendResult { Status = CurrencySpendStatus.InvalidAmount };
            }

            var normalizedCurrency = NormalizeCurrency(currency);
            if (normalizedCurrency == null)  // Entity not found — short-circuit with appropriate error result
            {
                return new CurrencySpendResult { Status = CurrencySpendStatus.UnsupportedCurrency };
            }

            // Keep the following dependent database writes in one transaction so a failure cannot persist partial state.
            await using var dbTransaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);  // Open serializable transaction — prevents race conditions on concurrent purchases

            var profile = await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId);  // Fetch single matching record or null if not found

            if (profile == null)  // Entity not found — short-circuit with appropriate error result
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

            await _context.PlayerCurrencyLogs.AddAsync(log);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            await dbTransaction.CommitAsync();  // Commit all staged changes atomically

            return new CurrencySpendResult
            {
                Status = CurrencySpendStatus.Success,
                PlayerProfile = profile,
                CurrencyLog = log,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter
            };
        }

        // Queries the database to retrieve normalize currency records.
        private static string? NormalizeCurrency(string? currency)
        {
            if (string.Equals(currency, "Gold", StringComparison.OrdinalIgnoreCase))
                return "Gold";

            if (string.Equals(currency, "Gems", StringComparison.OrdinalIgnoreCase))
                return "Gems";

            return null;
        }

        // Persists state modifications to the database for get balance.
        private static decimal GetBalance(PlayerProfile profile, string currency)
        {
            // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
            return currency == "Gold" ? profile.Gold : profile.Gems;
        }

        // Queries the database to retrieve set balance records.
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
