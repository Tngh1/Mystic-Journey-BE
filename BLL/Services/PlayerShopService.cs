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

            var playerExists = await _repository.PlayerExists(playerProfileId);
            if (!playerExists)
                throw new KeyNotFoundException("Player profile not found.");

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
                now);

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
            foreach (var dto in dtos)
            {
                purchasedToday.TryGetValue(dto.ShopItemId, out var purchasedCountToday);
                dto.PurchasedToday = purchasedCountToday;

                purchasedThisWeek.TryGetValue(dto.ShopItemId, out var purchasedCountWeek);
                dto.PurchasedThisWeek = purchasedCountWeek;

                ApplyAvailability(dto, now);
            }

            return new PagedResultDto<ShopItemPublicResponseDto>(totalCount, dtos);
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
                default:
                    throw new InvalidOperationException("Purchase failed.");
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
