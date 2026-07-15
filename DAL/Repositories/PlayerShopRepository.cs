using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Results;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DAL.Repositories
{
    public class PlayerShopRepository : IPlayerShopRepository
    {
        private const int DailyDealOfferCount = 10;

        private readonly MysticJourneyDbContext _context;

        public PlayerShopRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> PlayerExists(int playerProfileId)
        {
            return await _context.PlayerProfiles
                .AsNoTracking()
                .AnyAsync(p => p.PlayerProfileId == playerProfileId);
        }

        public async Task<PlayerShopRefreshState> GetOrCreateRefreshState(int playerProfileId, DateTime utcNow)
        {
            var (dayStart, _) = GetUtcDayRange(utcNow);

            var state = await _context.PlayerShopRefreshStates
                .FirstOrDefaultAsync(s =>
                    s.PlayerProfileId == playerProfileId &&
                    s.ShopDateUtc == dayStart);

            if (state != null)
                return state;

            state = new PlayerShopRefreshState
            {
                PlayerProfileId = playerProfileId,
                ShopDateUtc = dayStart,
                RefreshCount = 0,
                CreatedAt = utcNow,
                LastRefreshAt = utcNow
            };

            await _context.PlayerShopRefreshStates.AddAsync(state);
            try
            {
                await _context.SaveChangesAsync();
                return state;
            }
            catch (DbUpdateException)
            {
                _context.Entry(state).State = EntityState.Detached;

                var existingState = await _context.PlayerShopRefreshStates
                    .FirstOrDefaultAsync(s =>
                        s.PlayerProfileId == playerProfileId &&
                        s.ShopDateUtc == dayStart);

                if (existingState != null)
                    return existingState;

                throw;
            }
        }

        public async Task<PlayerShopRefreshState?> TryConsumeRefresh(
            int playerProfileId,
            DateTime utcNow,
            int maxDailyRefreshes)
        {
            await using var dbTransaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var (dayStart, _) = GetUtcDayRange(utcNow);

            var state = await _context.PlayerShopRefreshStates
                .FirstOrDefaultAsync(s =>
                    s.PlayerProfileId == playerProfileId &&
                    s.ShopDateUtc == dayStart);

            if (state == null)
            {
                state = new PlayerShopRefreshState
                {
                    PlayerProfileId = playerProfileId,
                    ShopDateUtc = dayStart,
                    RefreshCount = 0,
                    CreatedAt = utcNow,
                    LastRefreshAt = utcNow
                };

                await _context.PlayerShopRefreshStates.AddAsync(state);
                await _context.SaveChangesAsync();
            }

            if (state.RefreshCount >= maxDailyRefreshes)
            {
                await dbTransaction.RollbackAsync();
                return null;
            }

            state.RefreshCount += 1;
            state.LastRefreshAt = utcNow;

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return state;
        }

        public async Task<(int TotalCount, List<ShopItem> Items)> GetShopItems(
            int page,
            int pageSize,
            string? currency,
            string? itemType,
            string? search,
            bool includeSoldOut,
            DateTime utcNow,
            string shopSection,
            int? rotationSeed)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = BuildAvailableShopItemsQuery(
                shopSection,
                currency,
                itemType,
                search,
                includeSoldOut,
                utcNow);

            if (rotationSeed.HasValue)
            {
                var filteredItems = await query.ToListAsync();
                var rotatedItems = filteredItems
                    .OrderBy(s => GetStableRotationKey(rotationSeed.Value, s.ShopItemId))
                    .ThenBy(s => s.ShopItemId)
                    .Take(DailyDealOfferCount)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return (Math.Min(filteredItems.Count, DailyDealOfferCount), rotatedItems);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(s => s.Item != null ? s.Item.Type : string.Empty)
                .ThenBy(s => s.Currency)
                .ThenBy(s => s.Price)
                .ThenBy(s => s.ShopItemId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalCount, items);
        }


        public async Task<Dictionary<string, decimal>> GetFixedOriginalPrices(
            IEnumerable<(int ItemId, string Currency)> itemCurrencyPairs,
            DateTime utcNow)
        {
            var keys = itemCurrencyPairs
                .Select(k => new
                {
                    k.ItemId,
                    Currency = NormalizeCurrency(k.Currency) ?? k.Currency.Trim()
                })
                .Where(k => k.ItemId > 0 && !string.IsNullOrWhiteSpace(k.Currency))
                .Distinct()
                .ToList();

            if (keys.Count == 0)
                return new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            var itemIds = keys.Select(k => k.ItemId).Distinct().ToList();
            var keySet = keys
                .Select(k => BuildPriceKey(k.ItemId, k.Currency))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var fixedPrices = await _context.ShopItems
                .AsNoTracking()
                .Where(s =>
                    s.ShopSection == ShopSections.Fixed &&
                    s.IsActive &&
                    s.Item != null &&
                    s.Item.IsActive &&
                    itemIds.Contains(s.ItemId) &&
                    (!s.AvailableFrom.HasValue || s.AvailableFrom.Value <= utcNow) &&
                    (!s.AvailableTo.HasValue || s.AvailableTo.Value >= utcNow))
                .Select(s => new { s.ItemId, s.Currency, s.Price })
                .ToListAsync();

            return fixedPrices
                .Select(s => new
                {
                    Key = BuildPriceKey(s.ItemId, NormalizeCurrency(s.Currency) ?? s.Currency.Trim()),
                    s.Price
                })
                .Where(s => keySet.Contains(s.Key))
                .GroupBy(s => s.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Min(x => x.Price), StringComparer.OrdinalIgnoreCase);
        }
        public async Task<Dictionary<int, int>> GetPurchasedTodayCounts(
            int playerProfileId,
            IEnumerable<int> shopItemIds,
            DateTime utcNow)
        {
            var ids = shopItemIds.Distinct().ToList();
            if (ids.Count == 0)
                return new Dictionary<int, int>();

            var (dayStart, dayEnd) = GetUtcDayRange(utcNow);

            return await _context.PurchaseHistories
                .Where(p =>
                    p.PlayerProfileId == playerProfileId &&
                    ids.Contains(p.ShopItemId) &&
                    p.PurchasedAt >= dayStart &&
                    p.PurchasedAt < dayEnd)
                .GroupBy(p => p.ShopItemId)
                .Select(g => new { ShopItemId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToDictionaryAsync(x => x.ShopItemId, x => x.Quantity);
        }

        public async Task<Dictionary<int, int>> GetPurchasedThisWeekCounts(
            int playerProfileId,
            IEnumerable<int> shopItemIds,
            DateTime utcNow)
        {
            var ids = shopItemIds.Distinct().ToList();
            if (ids.Count == 0)
                return new Dictionary<int, int>();

            var (weekStart, weekEnd) = GetUtcWeekRange(utcNow);

            return await _context.PurchaseHistories
                .Where(p =>
                    p.PlayerProfileId == playerProfileId &&
                    ids.Contains(p.ShopItemId) &&
                    p.PurchasedAt >= weekStart &&
                    p.PurchasedAt < weekEnd)
                .GroupBy(p => p.ShopItemId)
                .Select(g => new { ShopItemId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToDictionaryAsync(x => x.ShopItemId, x => x.Quantity);
        }

        public async Task<PlayerShopPurchaseResult> PurchaseItem(
            int playerProfileId,
            int shopItemId,
            int quantity,
            DateTime utcNow)
        {
            if (quantity <= 0)
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.InvalidQuantity
                };
            }

            await using var dbTransaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var profile = await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId);

            if (profile == null)
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.PlayerNotFound
                };
            }

            var shopItem = await _context.ShopItems
                .Include(s => s.Item)
                .FirstOrDefaultAsync(s => s.ShopItemId == shopItemId);

            if (shopItem == null)
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.ShopItemNotFound,
                    PlayerProfile = profile
                };
            }

            var unavailableStatus = GetUnavailableStatus(shopItem, utcNow);
            if (unavailableStatus.HasValue)
            {
                return new PlayerShopPurchaseResult
                {
                    Status = unavailableStatus.Value,
                    PlayerProfile = profile,
                    ShopItem = shopItem
                };
            }

            if (IsDailyDeal(shopItem) && !await IsCurrentDailyDeal(playerProfileId, shopItem.ShopItemId, utcNow))
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.DailyDealNotAvailable,
                    PlayerProfile = profile,
                    ShopItem = shopItem
                };
            }

            if (shopItem.Stock >= 0 && shopItem.Stock < quantity)
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.SoldOut,
                    PlayerProfile = profile,
                    ShopItem = shopItem
                };
            }

            var purchasedToday = await GetPurchasedToday(playerProfileId, shopItemId, utcNow);
            if (shopItem.DailyPurchaseLimit > 0 &&
                purchasedToday + quantity > shopItem.DailyPurchaseLimit)
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.DailyLimitExceeded,
                    PlayerProfile = profile,
                    ShopItem = shopItem,
                    PurchasedTodayAfter = purchasedToday
                };
            }

            var purchasedThisWeek = await GetPurchasedThisWeek(playerProfileId, shopItemId, utcNow);
            if (shopItem.WeeklyPurchaseLimit > 0 &&
                purchasedThisWeek + quantity > shopItem.WeeklyPurchaseLimit)
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.WeeklyLimitExceeded,
                    PlayerProfile = profile,
                    ShopItem = shopItem,
                    PurchasedTodayAfter = purchasedToday,
                    PurchasedThisWeekAfter = purchasedThisWeek
                };
            }

            var currency = NormalizeCurrency(shopItem.Currency);
            if (currency == null)
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.UnsupportedCurrency,
                    PlayerProfile = profile,
                    ShopItem = shopItem
                };
            }

            var totalPrice = shopItem.Price * quantity;
            var balanceBefore = GetBalance(profile, currency);
            if (balanceBefore < totalPrice)
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.InsufficientCurrency,
                    PlayerProfile = profile,
                    ShopItem = shopItem,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = balanceBefore
                };
            }

            var balanceAfter = balanceBefore - totalPrice;
            SetBalance(profile, currency, balanceAfter);

            if (shopItem.Stock >= 0)
            {
                shopItem.Stock -= quantity;
            }

            var inventoryQuantity = await AddItemToInventory(playerProfileId, shopItem.ItemId, quantity);

            var purchase = new PurchaseHistory
            {
                PlayerProfileId = playerProfileId,
                ShopItemId = shopItem.ShopItemId,
                Quantity = quantity,
                TotalPrice = totalPrice,
                PurchasedAt = utcNow
            };

            var currencyLog = new PlayerCurrencyLog
            {
                PlayerProfileId = playerProfileId,
                Currency = currency,
                Type = "Spend",
                Amount = -totalPrice,
                BalanceAfter = balanceAfter,
                Note = $"Purchase shop item #{shopItem.ShopItemId}",
                CreatedAt = utcNow
            };

            await _context.PurchaseHistories.AddAsync(purchase);
            await _context.PlayerCurrencyLogs.AddAsync(currencyLog);
            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return new PlayerShopPurchaseResult
            {
                Status = PurchaseShopItemStatus.Success,
                PlayerProfile = profile,
                ShopItem = shopItem,
                PurchaseHistory = purchase,
                CurrencyLog = currencyLog,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter,
                InventoryQuantity = inventoryQuantity,
                PurchasedTodayAfter = purchasedToday + quantity,
                PurchasedThisWeekAfter = purchasedThisWeek + quantity
            };
        }

        private IQueryable<ShopItem> BuildAvailableShopItemsQuery(
            string shopSection,
            string? currency,
            string? itemType,
            string? search,
            bool includeSoldOut,
            DateTime utcNow)
        {
            var query = _context.ShopItems
                .Include(s => s.Item)
                .Where(s =>
                    s.ShopSection == shopSection &&
                    s.IsActive &&
                    s.Item != null &&
                    s.Item.IsActive &&
                    (!s.AvailableFrom.HasValue || s.AvailableFrom.Value <= utcNow) &&
                    (!s.AvailableTo.HasValue || s.AvailableTo.Value >= utcNow))
                .AsNoTracking();

            if (!includeSoldOut)
            {
                query = query.Where(s => s.Stock != 0);
            }

            if (!string.IsNullOrWhiteSpace(currency))
            {
                query = query.Where(s => s.Currency == currency);
            }

            if (!string.IsNullOrWhiteSpace(itemType))
            {
                query = query.Where(s => s.Item != null && s.Item.Type == itemType);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = $"%{search.Trim()}%";
                query = query.Where(s =>
                    s.Item != null &&
                    (EF.Functions.ILike(s.Item.Name, keyword) ||
                     (s.Item.Description != null && EF.Functions.ILike(s.Item.Description, keyword))));
            }

            return query;
        }

        private async Task<bool> IsCurrentDailyDeal(int playerProfileId, int shopItemId, DateTime utcNow)
        {
            var refreshState = await GetOrCreateRefreshState(playerProfileId, utcNow);
            var rotationSeed = BuildRotationSeed(
                playerProfileId,
                refreshState.ShopDateUtc,
                refreshState.RefreshCount);

            var pool = await BuildAvailableShopItemsQuery(
                    ShopSections.DailyDeal,
                    currency: null,
                    itemType: null,
                    search: null,
                    includeSoldOut: false,
                    utcNow)
                .ToListAsync();

            return pool
                .OrderBy(s => GetStableRotationKey(rotationSeed, s.ShopItemId))
                .ThenBy(s => s.ShopItemId)
                .Take(DailyDealOfferCount)
                .Any(s => s.ShopItemId == shopItemId);
        }

        private static PurchaseShopItemStatus? GetUnavailableStatus(ShopItem shopItem, DateTime utcNow)
        {
            if (!shopItem.IsActive)
                return PurchaseShopItemStatus.ShopItemInactive;

            if (shopItem.Item == null || !shopItem.Item.IsActive)
                return PurchaseShopItemStatus.ItemInactive;

            if (shopItem.AvailableFrom.HasValue && shopItem.AvailableFrom.Value > utcNow)
                return PurchaseShopItemStatus.NotYetAvailable;

            if (shopItem.AvailableTo.HasValue && shopItem.AvailableTo.Value < utcNow)
                return PurchaseShopItemStatus.Expired;

            return null;
        }

        private async Task<int> AddItemToInventory(int playerProfileId, int itemId, int quantity)
        {
            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.PlayerProfileId == playerProfileId && i.ItemId == itemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    PlayerProfileId = playerProfileId,
                    ItemId = itemId,
                    Quantity = quantity,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.InventoryItems.AddAsync(inventoryItem);
                return quantity;
            }

            inventoryItem.Quantity += quantity;
            return inventoryItem.Quantity;
        }

        private async Task<int> GetPurchasedToday(int playerProfileId, int shopItemId, DateTime utcNow)
        {
            var (dayStart, dayEnd) = GetUtcDayRange(utcNow);

            return await _context.PurchaseHistories
                .Where(p =>
                    p.PlayerProfileId == playerProfileId &&
                    p.ShopItemId == shopItemId &&
                    p.PurchasedAt >= dayStart &&
                    p.PurchasedAt < dayEnd)
                .SumAsync(p => (int?)p.Quantity) ?? 0;
        }

        private async Task<int> GetPurchasedThisWeek(int playerProfileId, int shopItemId, DateTime utcNow)
        {
            var (weekStart, weekEnd) = GetUtcWeekRange(utcNow);

            return await _context.PurchaseHistories
                .Where(p =>
                    p.PlayerProfileId == playerProfileId &&
                    p.ShopItemId == shopItemId &&
                    p.PurchasedAt >= weekStart &&
                    p.PurchasedAt < weekEnd)
                .SumAsync(p => (int?)p.Quantity) ?? 0;
        }

        private static (DateTime Start, DateTime End) GetUtcDayRange(DateTime utcNow)
        {
            var start = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
            return (start, start.AddDays(1));
        }

        private static (DateTime Start, DateTime End) GetUtcWeekRange(DateTime utcNow)
        {
            var start = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
            int diff = (7 + (start.DayOfWeek - DayOfWeek.Monday)) % 7;
            start = start.AddDays(-1 * diff);
            return (start, start.AddDays(7));
        }

        private static int BuildRotationSeed(int playerProfileId, DateTime shopDateUtc, int refreshCount)
        {
            unchecked
            {
                var seed = 17;
                seed = seed * 31 + playerProfileId;
                seed = seed * 31 + shopDateUtc.Year;
                seed = seed * 31 + shopDateUtc.Month;
                seed = seed * 31 + shopDateUtc.Day;
                seed = seed * 31 + refreshCount;
                return seed;
            }
        }

        private static int GetStableRotationKey(int seed, int shopItemId)
        {
            unchecked
            {
                uint hash = 2166136261;
                hash = (hash ^ (uint)seed) * 16777619;
                hash = (hash ^ (uint)shopItemId) * 16777619;
                return (int)(hash & 0x7fffffff);
            }
        }


        private static string BuildPriceKey(int itemId, string? currency)
        {
            var normalizedCurrency = NormalizeCurrency(currency) ?? currency?.Trim() ?? string.Empty;
            return $"{itemId}|{normalizedCurrency.ToUpperInvariant()}";
        }
        private static bool IsDailyDeal(ShopItem shopItem)
            => string.Equals(shopItem.ShopSection, ShopSections.DailyDeal, StringComparison.OrdinalIgnoreCase);

        private static string? NormalizeCurrency(string? currency)
        {
            if (string.Equals(currency, "Gold", StringComparison.OrdinalIgnoreCase))
                return "Gold";

            if (string.Equals(currency, "Gems", StringComparison.OrdinalIgnoreCase))
                return "Gems";

            return null;
        }

        private static decimal GetBalance(PlayerProfile profile, string currency)
            => currency == "Gold" ? profile.Gold : profile.Gems;

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