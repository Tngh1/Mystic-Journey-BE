using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ShopService : IShopService
    {
        private readonly IShopItemRepository _shopItemRepository;
        private readonly IPurchaseHistoryRepository _purchaseHistoryRepository;
        private readonly IPlayerProfileRepository _profileRepository;
        private readonly IInventoryService _inventoryService;
        private readonly IPlayerProfileService _playerProfileService;

        public ShopService(
            IShopItemRepository shopItemRepository,
            IPurchaseHistoryRepository purchaseHistoryRepository,
            IPlayerProfileRepository profileRepository,
            IInventoryService inventoryService,
            IPlayerProfileService playerProfileService)
        {
            _shopItemRepository = shopItemRepository;
            _purchaseHistoryRepository = purchaseHistoryRepository;
            _profileRepository = profileRepository;
            _inventoryService = inventoryService;
            _playerProfileService = playerProfileService;
        }

        public async Task<ShopListResponseDto> GetAllShopItemsAsync()
        {
            var items = await _shopItemRepository.GetAllActiveAsync();

            var dtos = items.Select(si => new ShopItemResponseDto
            {
                ShopItemId = si.Id,
                ItemId = si.ItemId,
                ItemName = si.Item?.Name ?? string.Empty,
                ItemDescription = si.Item?.Description,
                ItemType = si.Item?.Type.ToString() ?? string.Empty,
                ItemRarity = si.Item?.Rarity.ToString() ?? string.Empty,
                IconUrl = si.Item?.IconUrl,
                Currency = si.Currency.ToString(),
                Price = si.Price,
                Stock = si.Stock,
                DailyPurchaseLimit = si.DailyPurchaseLimit,
                IsActive = si.IsActive,
                AvailableFrom = si.AvailableFrom,
                AvailableTo = si.AvailableTo
            }).ToList();

            return new ShopListResponseDto
            {
                Success = true,
                Message = "Shop items retrieved successfully.",
                Items = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<ShopListResponseDto> GetAvailableItemsAsync()
        {
            var items = await _shopItemRepository.GetAvailableNowAsync();

            var dtos = items.Select(si => new ShopItemResponseDto
            {
                ShopItemId = si.Id,
                ItemId = si.ItemId,
                ItemName = si.Item?.Name ?? string.Empty,
                ItemDescription = si.Item?.Description,
                ItemType = si.Item?.Type.ToString() ?? string.Empty,
                ItemRarity = si.Item?.Rarity.ToString() ?? string.Empty,
                IconUrl = si.Item?.IconUrl,
                Currency = si.Currency.ToString(),
                Price = si.Price,
                Stock = si.Stock,
                DailyPurchaseLimit = si.DailyPurchaseLimit,
                IsActive = si.IsActive,
                AvailableFrom = si.AvailableFrom,
                AvailableTo = si.AvailableTo
            }).ToList();

            return new ShopListResponseDto
            {
                Success = true,
                Message = "Available shop items retrieved successfully.",
                Items = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<ShopApiResponseDto> GetShopItemByIdAsync(Guid shopItemId)
        {
            var shopItem = await _shopItemRepository.GetByIdWithItemAsync(shopItemId);

            if (shopItem == null)
            {
                return new ShopApiResponseDto
                {
                    Success = false,
                    Message = "Shop item not found."
                };
            }

            var dto = new ShopItemResponseDto
            {
                ShopItemId = shopItem.Id,
                ItemId = shopItem.ItemId,
                ItemName = shopItem.Item?.Name ?? string.Empty,
                ItemDescription = shopItem.Item?.Description,
                ItemType = shopItem.Item?.Type.ToString() ?? string.Empty,
                ItemRarity = shopItem.Item?.Rarity.ToString() ?? string.Empty,
                IconUrl = shopItem.Item?.IconUrl,
                Currency = shopItem.Currency.ToString(),
                Price = shopItem.Price,
                Stock = shopItem.Stock,
                DailyPurchaseLimit = shopItem.DailyPurchaseLimit,
                IsActive = shopItem.IsActive,
                AvailableFrom = shopItem.AvailableFrom,
                AvailableTo = shopItem.AvailableTo
            };

            return new ShopApiResponseDto
            {
                Success = true,
                Message = "Shop item retrieved successfully.",
                Item = dto
            };
        }

        public async Task<PurchaseApiResponseDto> PurchaseItemAsync(Guid accountId, PurchaseRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new PurchaseApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var shopItem = await _shopItemRepository.GetByIdAsync(request.ShopItemId);
            if (shopItem == null || !shopItem.IsActive)
            {
                return new PurchaseApiResponseDto
                {
                    Success = false,
                    Message = "Shop item not found or not available."
                };
            }

            var now = DateTime.UtcNow;
            if (shopItem.AvailableFrom.HasValue && shopItem.AvailableFrom > now)
            {
                return new PurchaseApiResponseDto
                {
                    Success = false,
                    Message = "This item is not available yet."
                };
            }

            if (shopItem.AvailableTo.HasValue && shopItem.AvailableTo < now)
            {
                return new PurchaseApiResponseDto
                {
                    Success = false,
                    Message = "This item is no longer available."
                };
            }

            var quantity = Math.Max(1, request.Quantity);

            if (shopItem.Stock >= 0 && shopItem.Stock < quantity)
            {
                return new PurchaseApiResponseDto
                {
                    Success = false,
                    Message = "Not enough stock available."
                };
            }

            if (shopItem.DailyPurchaseLimit > 0)
            {
                var dailyCount = await _purchaseHistoryRepository.GetDailyPurchaseCountAsync(profile.Id, shopItem.Id);
                if (dailyCount + quantity > shopItem.DailyPurchaseLimit)
                {
                    return new PurchaseApiResponseDto
                    {
                        Success = false,
                        Message = $"Daily purchase limit reached. You can only purchase {shopItem.DailyPurchaseLimit - dailyCount} more today."
                    };
                }
            }

            var totalPrice = shopItem.Price * quantity;

            if (shopItem.Currency == ShopItem.CurrencyType.Gold)
            {
                if (profile.Gold < totalPrice)
                {
                    return new PurchaseApiResponseDto
                    {
                        Success = false,
                        Message = $"Not enough gold. Required: {totalPrice}, Available: {profile.Gold}"
                    };
                }
                profile.Gold -= totalPrice;
            }
            else if (shopItem.Currency == ShopItem.CurrencyType.Gems)
            {
                if (profile.Gems < totalPrice)
                {
                    return new PurchaseApiResponseDto
                    {
                        Success = false,
                        Message = $"Not enough gems. Required: {totalPrice}, Available: {profile.Gems}"
                    };
                }
                profile.Gems -= totalPrice;
            }

            await _profileRepository.UpdateAsync(profile);

            var purchase = new PurchaseHistory
            {
                Id = Guid.NewGuid(),
                PlayerProfileId = profile.Id,
                ShopItemId = shopItem.Id,
                Quantity = quantity,
                TotalPrice = totalPrice,
                PurchasedAt = DateTime.UtcNow
            };

            await _purchaseHistoryRepository.CreateAsync(purchase);

            if (shopItem.Stock >= 0)
            {
                shopItem.Stock -= quantity;
                await _shopItemRepository.UpdateAsync(shopItem);
            }

            await _inventoryService.AddItemToInventoryAsync(accountId, new AddItemToInventoryRequestDto
            {
                ItemId = shopItem.ItemId,
                Quantity = quantity
            });

            var currencyResponse = new PlayerCurrencyResponseDto
            {
                Gold = profile.Gold,
                Gems = profile.Gems,
                Energy = profile.Energy,
                MaxEnergy = 100
            };

            return new PurchaseApiResponseDto
            {
                Success = true,
                Message = $"Successfully purchased {quantity}x {shopItem.Item?.Name}!",
                Currency = currencyResponse
            };
        }

        public async Task<PurchaseHistoryListResponseDto> GetPurchaseHistoryAsync(Guid accountId, int pageNumber = 1, int pageSize = 20)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new PurchaseHistoryListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var purchases = await _purchaseHistoryRepository.GetByPlayerProfileIdAsync(profile.Id, pageNumber, pageSize);
            var totalCount = await _purchaseHistoryRepository.GetTotalCountAsync(profile.Id);

            var dtos = purchases.Select(ph => new PurchaseHistoryResponseDto
            {
                PurchaseId = ph.Id,
                PlayerProfileId = ph.PlayerProfileId,
                ShopItemId = ph.ShopItemId,
                ItemName = ph.ShopItem?.Item?.Name ?? string.Empty,
                Quantity = ph.Quantity,
                TotalPrice = ph.TotalPrice,
                Currency = ph.ShopItem?.Currency.ToString() ?? string.Empty,
                PurchasedAt = ph.PurchasedAt
            }).ToList();

            return new PurchaseHistoryListResponseDto
            {
                Success = true,
                Message = "Purchase history retrieved successfully.",
                Purchases = dtos,
                TotalCount = totalCount
            };
        }
    }
}
