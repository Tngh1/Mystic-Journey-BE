using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Results;

namespace BLL.Services
{
    public class PlayerShopService : IPlayerShopService
    {
        private const int DailyDealOfferCount = 10;
        private const int MaxDailyRefreshes = 3;

        private readonly IPlayerShopRepository _repository;
        private readonly IMapper _mapper;

        public PlayerShopService(IPlayerShopRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<ShopItemPublicResponseDto>> GetShop(
            int playerProfileId,
            ViewShopQueryDto query)
        {
            EnsureAuthenticated(playerProfileId);
            await EnsurePlayerExists(playerProfileId);

            var page = Math.Max(1, query.Page);
            var pageSize = Math.Clamp(query.PageSize, 1, 100);
            var currency = NormalizeOptionalCurrency(query.Currency);
            var itemType = NormalizeOptionalText(query.ItemType);
            var search = NormalizeOptionalText(query.Search);
            var now = DateTime.UtcNow;

            var (totalCount, items) = await _repository.GetShopItems(
                page,
                pageSize,
                currency,
                itemType,
                search,
                query.IncludeSoldOut,
                now,
                ShopSections.Fixed,
                rotationSeed: null);

            return await MapShopResult(playerProfileId, totalCount, items, now);
        }

        public async Task<PagedResultDto<ShopItemPublicResponseDto>> GetDailyDeals(
            int playerProfileId,
            ViewShopQueryDto query)
        {
            EnsureAuthenticated(playerProfileId);
            await EnsurePlayerExists(playerProfileId);

            var now = DateTime.UtcNow;
            var refreshState = await _repository.GetOrCreateRefreshState(playerProfileId, now);
            var rotationSeed = BuildRotationSeed(
                playerProfileId,
                refreshState.ShopDateUtc,
                refreshState.RefreshCount);

            var (_, items) = await _repository.GetShopItems(
                page: 1,
                pageSize: DailyDealOfferCount,
                currency: null,
                itemType: null,
                search: null,
                includeSoldOut: query.IncludeSoldOut,
                utcNow: now,
                shopSection: ShopSections.DailyDeal,
                rotationSeed: rotationSeed);

            return await MapShopResult(playerProfileId, items.Count, items, now);
        }

        public async Task<ShopRefreshStatusDto> GetRefreshStatus(int playerProfileId)
        {
            EnsureAuthenticated(playerProfileId);
            await EnsurePlayerExists(playerProfileId);

            var now = DateTime.UtcNow;
            var refreshState = await _repository.GetOrCreateRefreshState(playerProfileId, now);
            return MapRefreshStatus(refreshState);
        }

        public Task<ShopRefreshResponseDto> RefreshShop(
            int playerProfileId,
            ViewShopQueryDto query)
            => RefreshDailyDeals(playerProfileId, query);

        public async Task<ShopRefreshResponseDto> RefreshDailyDeals(
            int playerProfileId,
            ViewShopQueryDto query)
        {
            EnsureAuthenticated(playerProfileId);
            await EnsurePlayerExists(playerProfileId);

            var now = DateTime.UtcNow;
            var refreshState = await _repository.TryConsumeRefresh(playerProfileId, now, MaxDailyRefreshes);
            if (refreshState == null)
                throw new BadRequestException("Daily shop refresh limit reached.");

            var shop = await GetDailyDeals(playerProfileId, query);
            return new ShopRefreshResponseDto
            {
                Success = true,
                Message = "Daily deals refreshed.",
                RefreshStatus = MapRefreshStatus(refreshState),
                Shop = shop
            };
        }

        public async Task<PurchaseShopItemResponseDto> PurchaseItem(
            int playerProfileId,
            PurchaseShopItemRequestDto request)
        {
            EnsureAuthenticated(playerProfileId);

            if (request.ShopItemId <= 0)
                throw new BadRequestException("Shop item ID must be greater than 0.");

            if (request.Quantity <= 0)
                throw new BadRequestException("Quantity must be greater than 0.");

            var result = await _repository.PurchaseItem(
                playerProfileId,
                request.ShopItemId,
                request.Quantity,
                DateTime.UtcNow);

            ThrowIfPurchaseFailed(result);

            var profile = result.PlayerProfile!;
            var shopItem = result.ShopItem!;
            var purchase = result.PurchaseHistory!;

            return new PurchaseShopItemResponseDto
            {
                Success = true,
                Message = "Item purchased.",
                PurchaseHistoryId = purchase.PurchaseHistoryId,
                ShopItemId = shopItem.ShopItemId,
                ItemId = shopItem.ItemId,
                ItemName = shopItem.Item?.Name ?? string.Empty,
                Quantity = purchase.Quantity,
                Currency = shopItem.Currency,
                UnitPrice = shopItem.Price,
                TotalPrice = purchase.TotalPrice,
                BalanceBefore = result.BalanceBefore,
                BalanceAfter = result.BalanceAfter,
                InventoryQuantity = result.InventoryQuantity,
                Balance = MapBalance(profile),
                Transaction = _mapper.Map<PlayerCurrencyLogResponseDto>(result.CurrencyLog)
            };
        }

        public async Task<IReadOnlyList<SkinShopItemResponseDto>> GetSkinShop(int playerProfileId)
        {
            EnsureAuthenticated(playerProfileId);
            var result = await _repository.GetSkinShop(playerProfileId);
            if (result.PlayerProfile == null)
                throw new KeyNotFoundException("Player profile not found.");

            return result.Skins.Select(skin =>
            {
                var isOwned = result.OwnedSkinIds.Contains(skin.SkinId);
                return new SkinShopItemResponseDto
                {
                    SkinId = skin.SkinId,
                    SkinName = skin.Name,
                    Description = skin.Description,
                    SkinType = skin.Type,
                    Rarity = skin.Rarity,
                    IconUrl = skin.IconUrl,
                    PreviewUrl = skin.PreviewUrl,
                    Currency = skin.Currency,
                    Price = skin.Price,
                    IsOwned = isOwned,
                    CanPurchase = !isOwned,
                    UnavailableReason = isOwned ? "Skin already owned." : null
                };
            }).ToList();
        }

        public async Task<PurchaseShopSkinResponseDto> PurchaseSkin(
            int playerProfileId,
            PurchaseShopSkinRequestDto request)
        {
            EnsureAuthenticated(playerProfileId);
            if (request.SkinId <= 0)
                throw new BadRequestException("Skin ID must be greater than 0.");

            var result = await _repository.PurchaseSkin(playerProfileId, request.SkinId, DateTime.UtcNow);
            ThrowIfSkinPurchaseFailed(result);

            var profile = result.PlayerProfile!;
            var skin = result.Skin!;
            return new PurchaseShopSkinResponseDto
            {
                Success = true,
                Message = "Skin purchased and unlocked.",
                PlayerSkinId = result.PlayerSkin!.PlayerSkinId,
                SkinId = skin.SkinId,
                SkinName = skin.Name,
                Currency = skin.Currency,
                Price = skin.Price,
                BalanceBefore = result.BalanceBefore,
                BalanceAfter = result.BalanceAfter,
                Balance = MapBalance(profile),
                Transaction = _mapper.Map<PlayerCurrencyLogResponseDto>(result.CurrencyLog)
            };
        }

        private async Task<PagedResultDto<ShopItemPublicResponseDto>> MapShopResult(
            int playerProfileId,
            int totalCount,
            List<ShopItem> items,
            DateTime now)
        {
            var shopItemIds = items.Select(x => x.ShopItemId).ToList();
            var purchasedToday = await _repository.GetPurchasedTodayCounts(
                playerProfileId,
                shopItemIds,
                now);

            var purchasedThisWeek = await _repository.GetPurchasedThisWeekCounts(
                playerProfileId,
                shopItemIds,
                now);

            var dtos = _mapper.Map<List<ShopItemPublicResponseDto>>(items);
            var originalPrices = await GetOriginalPriceLookup(items, now);
            foreach (var dto in dtos)
            {
                purchasedToday.TryGetValue(dto.ShopItemId, out var purchasedCountToday);
                dto.PurchasedToday = purchasedCountToday;

                purchasedThisWeek.TryGetValue(dto.ShopItemId, out var purchasedCountWeek);
                dto.PurchasedThisWeek = purchasedCountWeek;

                ApplyOriginalPrice(dto, originalPrices);
                ApplyAvailability(dto, now);
            }

            return new PagedResultDto<ShopItemPublicResponseDto>(totalCount, dtos);
        }


        private async Task<Dictionary<string, decimal>> GetOriginalPriceLookup(
            List<ShopItem> items,
            DateTime now)
        {
            var dailyDealPairs = items
                .Where(item => string.Equals(item.ShopSection, ShopSections.DailyDeal, StringComparison.OrdinalIgnoreCase))
                .Select(item => (item.ItemId, item.Currency))
                .ToList();

            if (dailyDealPairs.Count == 0)
                return new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            return await _repository.GetFixedOriginalPrices(dailyDealPairs, now);
        }

        private static void ApplyOriginalPrice(
            ShopItemPublicResponseDto item,
            IReadOnlyDictionary<string, decimal> originalPrices)
        {
            item.OriginalPrice = null;

            if (!string.Equals(item.ShopSection, ShopSections.DailyDeal, StringComparison.OrdinalIgnoreCase))
                return;

            if (!originalPrices.TryGetValue(BuildPriceKey(item.ItemId, item.Currency), out var originalPrice))
                return;

            if (originalPrice > item.Price)
                item.OriginalPrice = originalPrice;
        }

        private static string BuildPriceKey(int itemId, string? currency)
        {
            var normalizedCurrency = string.Equals(currency, "Gold", StringComparison.OrdinalIgnoreCase) ? "Gold"
                : string.Equals(currency, "Gems", StringComparison.OrdinalIgnoreCase) ? "Gems"
                : currency?.Trim() ?? string.Empty;

            return $"{itemId}|{normalizedCurrency.ToUpperInvariant()}";
        }

        private async Task EnsurePlayerExists(int playerProfileId)
        {
            var playerExists = await _repository.PlayerExists(playerProfileId);
            if (!playerExists)
                throw new KeyNotFoundException("Player profile not found.");
        }

        private static ShopRefreshStatusDto MapRefreshStatus(PlayerShopRefreshState state)
        {
            var used = Math.Clamp(state.RefreshCount, 0, MaxDailyRefreshes);
            var remaining = Math.Max(0, MaxDailyRefreshes - used);

            return new ShopRefreshStatusDto
            {
                ShopDateUtc = state.ShopDateUtc,
                NextResetUtc = state.ShopDateUtc.AddDays(1),
                RefreshesUsedToday = used,
                RefreshesRemainingToday = remaining,
                MaxDailyRefreshes = MaxDailyRefreshes,
                CanRefresh = remaining > 0
            };
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

        private static void ApplyAvailability(ShopItemPublicResponseDto item, DateTime utcNow)
        {
            item.IsUnlimitedStock = item.Stock < 0;
            item.RemainingDailyPurchases = item.DailyPurchaseLimit > 0
                ? Math.Max(0, item.DailyPurchaseLimit - item.PurchasedToday)
                : null;
            item.RemainingWeeklyPurchases = item.WeeklyPurchaseLimit > 0
                ? Math.Max(0, item.WeeklyPurchaseLimit - item.PurchasedThisWeek)
                : null;

            if (item.AvailableFrom.HasValue && item.AvailableFrom.Value > utcNow)
            {
                item.CanPurchase = false;
                item.UnavailableReason = "Shop item is not available yet.";
                return;
            }

            if (item.AvailableTo.HasValue && item.AvailableTo.Value < utcNow)
            {
                item.CanPurchase = false;
                item.UnavailableReason = "Shop item is expired.";
                return;
            }

            if (item.Stock == 0)
            {
                item.CanPurchase = false;
                item.UnavailableReason = "Sold out.";
                return;
            }

            if (item.DailyPurchaseLimit > 0 && item.RemainingDailyPurchases <= 0)
            {
                item.CanPurchase = false;
                item.UnavailableReason = "Daily purchase limit reached.";
                return;
            }

            if (item.WeeklyPurchaseLimit > 0 && item.RemainingWeeklyPurchases <= 0)
            {
                item.CanPurchase = false;
                item.UnavailableReason = "Weekly purchase limit reached.";
                return;
            }

            item.CanPurchase = true;
            item.UnavailableReason = null;
        }

        private static void ThrowIfPurchaseFailed(PlayerShopPurchaseResult result)
        {
            switch (result.Status)
            {
                case PurchaseShopItemStatus.Success:
                    return;
                case PurchaseShopItemStatus.PlayerNotFound:
                    throw new KeyNotFoundException("Player profile not found.");
                case PurchaseShopItemStatus.ShopItemNotFound:
                    throw new KeyNotFoundException("Shop item not found.");
                case PurchaseShopItemStatus.InvalidQuantity:
                    throw new BadRequestException("Quantity must be greater than 0.");
                case PurchaseShopItemStatus.ShopItemInactive:
                case PurchaseShopItemStatus.ItemInactive:
                    throw new BadRequestException("Shop item is not available.");
                case PurchaseShopItemStatus.NotYetAvailable:
                    throw new BadRequestException("Shop item is not available yet.");
                case PurchaseShopItemStatus.Expired:
                    throw new BadRequestException("Shop item is expired.");
                case PurchaseShopItemStatus.SoldOut:
                    throw new BadRequestException("Shop item is sold out.");
                case PurchaseShopItemStatus.DailyLimitExceeded:
                    throw new BadRequestException("Daily purchase limit exceeded.");
                case PurchaseShopItemStatus.WeeklyLimitExceeded:
                    throw new BadRequestException("Weekly purchase limit exceeded.");
                case PurchaseShopItemStatus.UnsupportedCurrency:
                    throw new BadRequestException("Currency must be Gold or Gems.");
                case PurchaseShopItemStatus.InsufficientCurrency:
                    throw new BadRequestException($"Not enough {result.ShopItem?.Currency ?? "currency"}.");
                case PurchaseShopItemStatus.DailyDealNotAvailable:
                    throw new BadRequestException("Daily deal is not available in your current daily shop.");
                default:
                    throw new InvalidOperationException("Purchase failed.");
            }
        }

        private static void ThrowIfSkinPurchaseFailed(PlayerShopSkinPurchaseResult result)
        {
            switch (result.Status)
            {
                case PurchaseShopSkinStatus.Success:
                    return;
                case PurchaseShopSkinStatus.PlayerNotFound:
                    throw new KeyNotFoundException("Player profile not found.");
                case PurchaseShopSkinStatus.SkinNotFound:
                    throw new KeyNotFoundException("Skin not found.");
                case PurchaseShopSkinStatus.WrongClass:
                    throw new BadRequestException("This skin is not available for your class.");
                case PurchaseShopSkinStatus.NotForSale:
                    throw new BadRequestException("This skin is not for sale.");
                case PurchaseShopSkinStatus.AlreadyOwned:
                    throw new BadRequestException("Skin already owned.");
                case PurchaseShopSkinStatus.UnsupportedCurrency:
                    throw new BadRequestException("Currency must be Gold or Gems.");
                case PurchaseShopSkinStatus.InsufficientCurrency:
                    throw new BadRequestException($"Not enough {result.Skin?.Currency ?? "currency"}.");
                default:
                    throw new InvalidOperationException("Skin purchase failed.");
            }
        }

        private static CurrencyBalanceResponseDto MapBalance(PlayerProfile profile)
        {
            return new CurrencyBalanceResponseDto
            {
                PlayerProfileId = profile.PlayerProfileId,
                Gold = profile.Gold,
                Gems = profile.Gems,
                ServerTimeUtc = DateTime.UtcNow
            };
        }

        private static void EnsureAuthenticated(int playerProfileId)
        {
            if (playerProfileId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");
        }

        private static string? NormalizeOptionalCurrency(string? currency)
            => string.IsNullOrWhiteSpace(currency)
                ? null
                : string.Equals(currency, "Gold", StringComparison.OrdinalIgnoreCase) ? "Gold"
                : string.Equals(currency, "Gems", StringComparison.OrdinalIgnoreCase) ? "Gems"
                : throw new BadRequestException("Currency must be Gold or Gems.");

        private static string? NormalizeOptionalText(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
