using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Results;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DAL.Repositories
{
    // Queries the database to retrieve i player shop repository records.
    public class PlayerShopRepository : IPlayerShopRepository
    {
        private const int DailyDealOfferCount = 10;

        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of PlayerShopRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PlayerShopRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Checks whether a player profile record exists in the database for the given ID without tracking.
        public async Task<bool> PlayerExists(int playerProfileId)
        {
            return await _context.PlayerProfiles
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .AnyAsync(p => p.PlayerProfileId == playerProfileId);  // Check existence without loading the full entity
        }

        // Retrieves daily shop refresh state for the player, or creates a new initial record if none exists today.
        public async Task<PlayerShopRefreshState> GetOrCreateRefreshState(int playerProfileId, DateTime utcNow)
        {
            var (dayStart, _) = GetUtcDayRange(utcNow);

            var state = await _context.PlayerShopRefreshStates
                .FirstOrDefaultAsync(s =>  // Fetch single matching record or null if not found
                    s.PlayerProfileId == playerProfileId &&
                    s.ShopDateUtc == dayStart);

            if (state != null)  // Entity exists — proceed with conditional branch
                return state;

            state = new PlayerShopRefreshState
            {
                PlayerProfileId = playerProfileId,
                ShopDateUtc = dayStart,
                RefreshCount = 0,
                CreatedAt = utcNow,
                LastRefreshAt = utcNow
            };

            await _context.PlayerShopRefreshStates.AddAsync(state);  // Stage new entity for insertion in the next SaveChanges call
            try
            {
                await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
                return state;
            }
            catch (DbUpdateException)
            {
                _context.Entry(state).State = EntityState.Detached;

                var existingState = await _context.PlayerShopRefreshStates
                    .FirstOrDefaultAsync(s =>  // Fetch single matching record or null if not found
                        s.PlayerProfileId == playerProfileId &&
                        s.ShopDateUtc == dayStart);

                if (existingState != null)  // Entity exists — proceed with conditional branch
                    return existingState;

                throw;
            }
        }

        // Attempt consume refresh using player profile id, utc now, and max daily refreshes; it opens a database transaction, loads utc day range, selects the matching record, creates async, and updates changes async and guards invalid or unavailable states and keeps dependent writes atomic.
        public async Task<PlayerShopRefreshState?> TryConsumeRefresh(
            int playerProfileId,
            DateTime utcNow,
            int maxDailyRefreshes)
        {
            // Keep the following dependent database writes in one transaction so a failure cannot persist partial state.
            await using var dbTransaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);  // Open serializable transaction — prevents race conditions on concurrent purchases
            var (dayStart, _) = GetUtcDayRange(utcNow);

            var state = await _context.PlayerShopRefreshStates
                .FirstOrDefaultAsync(s =>  // Fetch single matching record or null if not found
                    s.PlayerProfileId == playerProfileId &&
                    s.ShopDateUtc == dayStart);

            if (state == null)  // Entity not found — short-circuit with appropriate error result
            {
                state = new PlayerShopRefreshState
                {
                    PlayerProfileId = playerProfileId,
                    ShopDateUtc = dayStart,
                    RefreshCount = 0,
                    CreatedAt = utcNow,
                    LastRefreshAt = utcNow
                };

                await _context.PlayerShopRefreshStates.AddAsync(state);  // Stage new entity for insertion in the next SaveChanges call
                await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            }

            if (state.RefreshCount >= maxDailyRefreshes)
            {
                await dbTransaction.RollbackAsync();  // Roll back partial writes to maintain data integrity
                return null;
            }

            state.RefreshCount += 1;
            state.LastRefreshAt = utcNow;

            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            await dbTransaction.CommitAsync();  // Commit all staged changes atomically

            return state;
        }

        // Process the supplied values: maps the input discriminator to the corresponding domain value and fallback.
        public async Task<(int TotalCount, List<ShopItem> Items)> GetShopItems(
            int page,
            int pageSize,
            // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
            string? currency,
            // Supported item types: Weapon, Armor, Consumable, Material, QuestItem, or Currency; the type controls filtering, stacking, and usage behavior.
            string? itemType,
            string? search,
            bool includeSoldOut,
            DateTime utcNow,
            string shopSection,
            int? rotationSeed)
        {
            page = Math.Max(1, page);
            // Clamp the calculated value to the minimum and maximum accepted by this domain rule.
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
                var filteredItems = await query.ToListAsync();  // Materialize the query into a list from the database
                var rotatedItems = filteredItems
                    .OrderBy(s => GetStableRotationKey(rotationSeed.Value, s.ShopItemId))  // Sort results oldest/lowest first
                    .ThenBy(s => s.ShopItemId)
                    .Take(DailyDealOfferCount)  // Apply pagination limit — cap result set size
                    .Skip((page - 1) * pageSize)  // Apply pagination offset — skip already-seen records
                    .Take(pageSize)  // Apply pagination limit — cap result set size
                    .ToList();

                return (Math.Min(filteredItems.Count, DailyDealOfferCount), rotatedItems);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(s => s.Item != null ? s.Item.Type : string.Empty)  // Sort results oldest/lowest first
                .ThenBy(s => s.Currency)
                .ThenBy(s => s.Price)
                .ThenBy(s => s.ShopItemId)
                .Skip((page - 1) * pageSize)  // Apply pagination offset — skip already-seen records
                .Take(pageSize)  // Apply pagination limit — cap result set size
                .ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }


        // Process the supplied values: normalizes or validates the text before returning the derived result and maps the input discriminator to the corresponding domain value and fallback.
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
                .Where(k => k.ItemId > 0 && !string.IsNullOrWhiteSpace(k.Currency))  // Filter records matching the predicate
                .Distinct()
                .ToList();

            if (keys.Count == 0)
                return new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            var itemIds = keys.Select(k => k.ItemId).Distinct().ToList();
            var keySet = keys
                .Select(k => BuildPriceKey(k.ItemId, k.Currency))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var fixedPrices = await _context.ShopItems
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Where(s =>  // Filter records matching the predicate
                    s.ShopSection == ShopSections.Fixed &&
                    s.IsActive &&
                    s.Item != null &&
                    s.Item.IsActive &&
                    itemIds.Contains(s.ItemId) &&
                    (!s.AvailableFrom.HasValue || s.AvailableFrom.Value <= utcNow) &&
                    (!s.AvailableTo.HasValue || s.AvailableTo.Value >= utcNow))
                .Select(s => new { s.ItemId, s.Currency, s.Price })
                .ToListAsync();  // Materialize the query into a list from the database

            return fixedPrices
                .Select(s => new
                {
                    Key = BuildPriceKey(s.ItemId, NormalizeCurrency(s.Currency) ?? s.Currency.Trim()),
                    s.Price
                })
                .Where(s => keySet.Contains(s.Key))  // Filter records matching the predicate
                .GroupBy(s => s.Key, StringComparer.OrdinalIgnoreCase)  // Aggregate records by grouping key
                .ToDictionary(g => g.Key, g => g.Min(x => x.Price), StringComparer.OrdinalIgnoreCase);
        }
        // Count the loaded records by their category key and return a lookup used to render filter totals.
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
                .Where(p =>  // Filter records matching the predicate
                    p.PlayerProfileId == playerProfileId &&
                    ids.Contains(p.ShopItemId) &&
                    p.PurchasedAt >= dayStart &&
                    p.PurchasedAt < dayEnd)
                .GroupBy(p => p.ShopItemId)  // Aggregate records by grouping key
                .Select(g => new { ShopItemId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToDictionaryAsync(x => x.ShopItemId, x => x.Quantity);
        }

        // Count the loaded records by their category key and return a lookup used to render filter totals.
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
                .Where(p =>  // Filter records matching the predicate
                    p.PlayerProfileId == playerProfileId &&
                    ids.Contains(p.ShopItemId) &&
                    p.PurchasedAt >= weekStart &&
                    p.PurchasedAt < weekEnd)
                .GroupBy(p => p.ShopItemId)  // Aggregate records by grouping key
                .Select(g => new { ShopItemId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToDictionaryAsync(x => x.ShopItemId, x => x.Quantity);
        }

        // Process purchase item using player profile id, shop item id, quantity, and utc now; it selects the matching record, loads unavailable status, loads purchased today, loads purchased this week, and creates async and guards invalid or unavailable states and keeps dependent writes atomic.
        public async Task<PlayerShopPurchaseResult> PurchaseItem(
            int playerProfileId,
            int shopItemId,
            int quantity,
            DateTime utcNow)
        {
            if (quantity <= 0)  // Reject zero or negative item quantities before any DB work
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.InvalidQuantity
                };
            }

            // Keep the following dependent database writes in one transaction so a failure cannot persist partial state.
            await using var dbTransaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);  // Open serializable transaction — prevents race conditions on concurrent purchases

            var profile = await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId);  // Fetch single matching record or null if not found

            if (profile == null)  // Entity not found — short-circuit with appropriate error result
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.PlayerNotFound
                };
            }

            var shopItem = await _context.ShopItems
                .Include(s => s.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(s => s.ShopItemId == shopItemId);  // Fetch single matching record or null if not found

            if (shopItem == null)  // Entity not found — short-circuit with appropriate error result
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.ShopItemNotFound,
                    PlayerProfile = profile
                };
            }

            var unavailableStatus = GetUnavailableStatus(shopItem, utcNow); // Check if item is expired, not yet active, or otherwise unavailable
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
                // This item is a daily deal but player's daily deal slot doesn't match this item today
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.DailyDealNotAvailable,
                    PlayerProfile = profile,
                    ShopItem = shopItem
                };
            }

            if (shopItem.Stock >= 0 && shopItem.Stock < quantity) // Requested quantity exceeds remaining stock
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.SoldOut,
                    PlayerProfile = profile,
                    ShopItem = shopItem
                };
            }

            var purchasedToday = await GetPurchasedToday(playerProfileId, shopItemId, utcNow); // Query how many units already purchased today
            if (shopItem.DailyPurchaseLimit > 0 &&
                purchasedToday + quantity > shopItem.DailyPurchaseLimit) // Adding this quantity would exceed the daily purchase cap
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.DailyLimitExceeded,
                    PlayerProfile = profile,
                    ShopItem = shopItem,
                    PurchasedTodayAfter = purchasedToday
                };
            }

            var purchasedThisWeek = await GetPurchasedThisWeek(playerProfileId, shopItemId, utcNow); // Query how many units purchased this calendar week
            if (shopItem.WeeklyPurchaseLimit > 0 &&
                purchasedThisWeek + quantity > shopItem.WeeklyPurchaseLimit) // Adding this quantity would exceed the weekly purchase cap
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

            // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
            var currency = NormalizeCurrency(shopItem.Currency); // Resolve currency string ('Gold'/'Gem') to canonical enum
            if (currency == null) // Currency type is not supported — reject purchase
            {
                return new PlayerShopPurchaseResult
                {
                    Status = PurchaseShopItemStatus.UnsupportedCurrency,
                    PlayerProfile = profile,
                    ShopItem = shopItem
                };
            }

            var totalPrice = shopItem.Price * quantity; // Calculate total cost before deducting balance
            var balanceBefore = GetBalance(profile, currency); // Read current player wallet balance for this currency
            if (balanceBefore < totalPrice) // Insufficient balance — reject purchase without modifying state
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

            var balanceAfter = balanceBefore - totalPrice; // Deduct price from balance
            SetBalance(profile, currency, balanceAfter); // Write new balance back to the player profile entity

            if (shopItem.Stock >= 0)
            {
                shopItem.Stock -= quantity; // Decrement physical stock counter for limited-supply items
            }

            var inventoryQuantity = await AddItemToInventory(playerProfileId, shopItem.ItemId, quantity); // Add purchased item to inventory, returning new total stack count

            var purchase = new PurchaseHistory // Build purchase history record for audit trail
            {
                PlayerProfileId = playerProfileId,
                ShopItemId = shopItem.ShopItemId,
                Quantity = quantity,
                TotalPrice = totalPrice,
                PurchasedAt = utcNow
            };

            var currencyLog = new PlayerCurrencyLog // Build currency deduction log for financial audit
            {
                PlayerProfileId = playerProfileId,
                Currency = currency,
                Type = "Spend",
                Amount = -totalPrice,
                BalanceAfter = balanceAfter,
                Note = $"Purchase shop item #{shopItem.ShopItemId}",
                CreatedAt = utcNow
            };

            await _context.PurchaseHistories.AddAsync(purchase); // Stage purchase history insert for this transaction
            await _context.PlayerCurrencyLogs.AddAsync(currencyLog); // Stage currency log insert for this transaction
            await _context.SaveChangesAsync(); // Flush all staged changes to DB within the open transaction
            await dbTransaction.CommitAsync(); // Commit transaction — all writes become visible atomically

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

        // Queries the database to retrieve get skin shop records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the matching PlayerSkinShopResult entity result or default if not found.
        public async Task<PlayerSkinShopResult> GetSkinShop(int playerProfileId)
        {
            var profile = await _context.PlayerProfiles
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId);  // Fetch single matching record or null if not found

            if (profile == null)  // Entity not found — short-circuit with appropriate error result
                return new PlayerSkinShopResult();

            var skinId = GetPremiumSkinId(profile.Class);
            var skins = skinId.HasValue
                ? await _context.Skins
                    // Execute this query without change tracking because the returned entities are read-only.
                    .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                    .Where(s => s.SkinId == skinId.Value && s.IsActive && s.IsForSale)  // Filter records matching the predicate
                    .ToListAsync()  // Materialize the query into a list from the database
                : new List<Skin>();

            var ownedSkinIds = (await _context.PlayerSkins
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Where(ps => ps.PlayerProfileId == playerProfileId)  // Filter records matching the predicate
                .Select(ps => ps.SkinId)
                .ToListAsync()).ToHashSet();  // Materialize the query into a list from the database

            return new PlayerSkinShopResult
            {
                PlayerProfile = profile,
                Skins = skins,
                OwnedSkinIds = ownedSkinIds
            };
        }

        // Process purchase skin using player profile id, skin id, and utc now; it selects the matching record, loads premium skin id, checks whether a matching record exists, loads balance, and creates async and guards invalid or unavailable states and keeps dependent writes atomic.
        public async Task<PlayerShopSkinPurchaseResult> PurchaseSkin(
            int playerProfileId,
            int skinId,
            DateTime utcNow)
        {
            // Keep the following dependent database writes in one transaction so a failure cannot persist partial state.
            await using var dbTransaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);  // Open serializable transaction — prevents race conditions on concurrent purchases

            var profile = await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId);  // Fetch single matching record or null if not found
            if (profile == null)  // Entity not found — short-circuit with appropriate error result
                return new PlayerShopSkinPurchaseResult { Status = PurchaseShopSkinStatus.PlayerNotFound };

            var skin = await _context.Skins.FirstOrDefaultAsync(s => s.SkinId == skinId);  // Fetch single matching record or null if not found
            if (skin == null)  // Entity not found — short-circuit with appropriate error result
                return new PlayerShopSkinPurchaseResult
                {
                    Status = PurchaseShopSkinStatus.SkinNotFound,
                    PlayerProfile = profile
                };

            if (GetPremiumSkinId(profile.Class) != skinId)
                return new PlayerShopSkinPurchaseResult
                {
                    Status = PurchaseShopSkinStatus.WrongClass,
                    PlayerProfile = profile,
                    Skin = skin
                };

            if (!skin.IsActive || !skin.IsForSale)
                return new PlayerShopSkinPurchaseResult
                {
                    Status = PurchaseShopSkinStatus.NotForSale,
                    PlayerProfile = profile,
                    Skin = skin
                };

            if (await _context.PlayerSkins.AnyAsync(ps => ps.PlayerProfileId == playerProfileId && ps.SkinId == skinId))  // Check existence without loading the full entity
                return new PlayerShopSkinPurchaseResult
                {
                    Status = PurchaseShopSkinStatus.AlreadyOwned,
                    PlayerProfile = profile,
                    Skin = skin
                };

            // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
            var currency = NormalizeCurrency(skin.Currency);
            if (currency == null)  // Entity not found — short-circuit with appropriate error result
                return new PlayerShopSkinPurchaseResult
                {
                    Status = PurchaseShopSkinStatus.UnsupportedCurrency,
                    PlayerProfile = profile,
                    Skin = skin
                };

            var balanceBefore = GetBalance(profile, currency);
            if (balanceBefore < skin.Price)
                return new PlayerShopSkinPurchaseResult
                {
                    Status = PurchaseShopSkinStatus.InsufficientCurrency,
                    PlayerProfile = profile,
                    Skin = skin,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = balanceBefore
                };

            var balanceAfter = balanceBefore - skin.Price;
            SetBalance(profile, currency, balanceAfter);

            var playerSkin = new PlayerSkin
            {
                PlayerProfileId = playerProfileId,
                SkinId = skinId,
                IsEquipped = false,
                UnlockedAt = utcNow
            };
            var currencyLog = new PlayerCurrencyLog
            {
                PlayerProfileId = playerProfileId,
                Currency = currency,
                Type = "Spend",
                Amount = -skin.Price,
                BalanceAfter = balanceAfter,
                Note = $"Purchase skin #{skinId}",
                CreatedAt = utcNow
            };

            await _context.PlayerSkins.AddAsync(playerSkin);  // Stage new entity for insertion in the next SaveChanges call
            await _context.PlayerCurrencyLogs.AddAsync(currencyLog);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            await dbTransaction.CommitAsync();  // Commit all staged changes atomically

            return new PlayerShopSkinPurchaseResult
            {
                Status = PurchaseShopSkinStatus.Success,
                PlayerProfile = profile,
                Skin = skin,
                PlayerSkin = playerSkin,
                CurrencyLog = currencyLog,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter
            };
        }

        // Process the supplied values: normalizes or validates the text before returning the derived result.
        private IQueryable<ShopItem> BuildAvailableShopItemsQuery(
            string shopSection,
            // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
            string? currency,
            // Supported item types: Weapon, Armor, Consumable, Material, QuestItem, or Currency; the type controls filtering, stacking, and usage behavior.
            string? itemType,
            string? search,
            bool includeSoldOut,
            DateTime utcNow)
        {
            var query = _context.ShopItems
                .Include(s => s.Item!)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(i => i!.EquipmentStats)
                .Where(s =>  // Filter records matching the predicate
                    s.ShopSection == shopSection &&
                    s.IsActive &&
                    s.Item != null &&
                    s.Item.IsActive &&
                    (!s.AvailableFrom.HasValue || s.AvailableFrom.Value <= utcNow) &&
                    (!s.AvailableTo.HasValue || s.AvailableTo.Value >= utcNow))
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!includeSoldOut)
            {
                query = query.Where(s => s.Stock != 0);  // Filter records matching the predicate
            }

            if (!string.IsNullOrWhiteSpace(currency))
            {
                query = query.Where(s => s.Currency == currency);  // Filter records matching the predicate
            }

            if (!string.IsNullOrWhiteSpace(itemType))
            {
                query = query.Where(s => s.Item != null && s.Item.Type == itemType);  // Filter records matching the predicate
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = $"%{search.Trim()}%";
                query = query.Where(s =>  // Filter records matching the predicate
                    s.Item != null &&
                    (EF.Functions.ILike(s.Item.Name, keyword) ||
                     (s.Item.Description != null && EF.Functions.ILike(s.Item.Description, keyword))));
            }

            return query;
        }

        // Queries the database to retrieve is current daily deal records.
        // Query details: sorts records according to business ordering rules.
        // Returns true if the operation succeeded or record exists; otherwise false.
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
                .ToListAsync();  // Materialize the query into a list from the database

            return pool
                .OrderBy(s => GetStableRotationKey(rotationSeed, s.ShopItemId))  // Sort results oldest/lowest first
                .ThenBy(s => s.ShopItemId)
                .Take(DailyDealOfferCount)  // Apply pagination limit — cap result set size
                .Any(s => s.ShopItemId == shopItemId);
        }

        // Performs database query and transactional persistence workflow for get unavailable status.
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

        // Performs database query and transactional persistence workflow for add item to inventory.
        // Returns the computed numeric count or database ID result.
        private async Task<int> AddItemToInventory(int playerProfileId, int itemId, int quantity)
        {
            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.PlayerProfileId == playerProfileId && i.ItemId == itemId);  // Fetch single matching record or null if not found

            if (inventoryItem == null)  // Entity not found — short-circuit with appropriate error result
            {
                inventoryItem = new InventoryItem
                {
                    PlayerProfileId = playerProfileId,
                    ItemId = itemId,
                    Quantity = quantity,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.InventoryItems.AddAsync(inventoryItem);  // Stage new entity for insertion in the next SaveChanges call
                return quantity;
            }

            inventoryItem.Quantity += quantity;
            return inventoryItem.Quantity;
        }

        // Queries the database to retrieve get purchased today records.
        // Returns the computed numeric count or database ID result.
        private async Task<int> GetPurchasedToday(int playerProfileId, int shopItemId, DateTime utcNow)
        {
            var (dayStart, dayEnd) = GetUtcDayRange(utcNow);

            return await _context.PurchaseHistories
                .Where(p =>  // Filter records matching the predicate
                    p.PlayerProfileId == playerProfileId &&
                    p.ShopItemId == shopItemId &&
                    p.PurchasedAt >= dayStart &&
                    p.PurchasedAt < dayEnd)
                .SumAsync(p => (int?)p.Quantity) ?? 0;
        }

        // Queries the database to retrieve get purchased this week records.
        // Returns the computed numeric count or database ID result.
        private async Task<int> GetPurchasedThisWeek(int playerProfileId, int shopItemId, DateTime utcNow)
        {
            var (weekStart, weekEnd) = GetUtcWeekRange(utcNow);

            return await _context.PurchaseHistories
                .Where(p =>  // Filter records matching the predicate
                    p.PlayerProfileId == playerProfileId &&
                    p.ShopItemId == shopItemId &&
                    p.PurchasedAt >= weekStart &&
                    p.PurchasedAt < weekEnd)
                .SumAsync(p => (int?)p.Quantity) ?? 0;
        }

        // Queries the database to retrieve static records.
        private static (DateTime Start, DateTime End) GetUtcDayRange(DateTime utcNow)
        {
            var start = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
            return (start, start.AddDays(1));
        }

        // Queries the database to retrieve static records.
        private static (DateTime Start, DateTime End) GetUtcWeekRange(DateTime utcNow)
        {
            var start = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
            int diff = (7 + (start.DayOfWeek - DayOfWeek.Monday)) % 7;
            start = start.AddDays(-1 * diff);
            return (start, start.AddDays(7));
        }

        // Queries the database to retrieve build rotation seed records.
        // Returns the computed numeric count or database ID result.
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

        // Queries the database to retrieve get stable rotation key records.
        // Returns the computed numeric count or database ID result.
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


        // Queries the database to retrieve build price key records.
        private static string BuildPriceKey(int itemId, string? currency)
        {
            var normalizedCurrency = NormalizeCurrency(currency) ?? currency?.Trim() ?? string.Empty;
            return $"{itemId}|{normalizedCurrency.ToUpperInvariant()}";
        }
        // Queries the database to retrieve is daily deal records.
        // Returns true if the operation succeeded or record exists; otherwise false.
        private static bool IsDailyDeal(ShopItem shopItem)
            => string.Equals(shopItem.ShopSection, ShopSections.DailyDeal, StringComparison.OrdinalIgnoreCase);

        // Queries the database to retrieve normalize currency records.
        private static string? NormalizeCurrency(string? currency)
        {
            if (string.Equals(currency, "Gold", StringComparison.OrdinalIgnoreCase))
                return "Gold";

            if (string.Equals(currency, "Gems", StringComparison.OrdinalIgnoreCase))
                return "Gems";

            return null;
        }

        // Queries the database to retrieve get premium skin id records.
        private static int? GetPremiumSkinId(string? playerClass)
            => playerClass?.Trim().ToLowerInvariant() switch
            {
                "archer" => 4,
                "knight" => 5,
                "mage" => 6,
                _ => null
            };

        // Persists state modifications to the database for get balance.
        private static decimal GetBalance(PlayerProfile profile, string currency)
            => currency == "Gold" ? profile.Gold : profile.Gems;

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
