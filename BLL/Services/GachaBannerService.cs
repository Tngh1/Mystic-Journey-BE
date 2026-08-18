using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    // Executes core business logic for i gacha banner service.
    public class GachaBannerService : IGachaBannerService
    {
        private readonly IGachaBannerRepository _repository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly IMapper _mapper;
        private readonly IRewardDeliveryService _rewardDeliveryService;

        // Initialize this instance from repository, player profile repository, inventory repository, and item repository and store repository, player profile repository, inventory repository, item repository, and transaction manager for later operations.
        public GachaBannerService(
            IGachaBannerRepository repository,
            IPlayerProfileRepository playerProfileRepository,
            IInventoryRepository inventoryRepository,
            IItemRepository itemRepository,
            ITransactionManager transactionManager,
            IMapper mapper,
            IRewardDeliveryService rewardDeliveryService)
        {
            _repository = repository;
            _playerProfileRepository = playerProfileRepository;
            _inventoryRepository = inventoryRepository;
            _itemRepository = itemRepository;
            _transactionManager = transactionManager;
            _mapper = mapper;
            _rewardDeliveryService = rewardDeliveryService;
        }

        // Executes core business logic for validate cost item.
        // Logic details: validates numeric boundary constraints; delegates data queries and updates to repository layer; throws KeyNotFoundException, ArgumentException on invalid state or rule violations.
        // Completes asynchronously upon successful execution.
        private async Task ValidateCostItem(int pullCost, int? costItemId)
        {
            if (pullCost <= 0)
                return;

            if (!costItemId.HasValue)
                throw new ArgumentException(
                    "CostItemId is required when PullCost > 0. A gacha pull must be paid with a ticket item; Coin, Gem and Energy are not accepted.");

            var item = await _itemRepository.GetItemById(costItemId.Value)
                ?? throw new KeyNotFoundException($"Cost item with id {costItemId.Value} not found.");

            if (string.Equals(item.Type, "Currency", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(
                    $"Item '{item.Name}' is a Currency item and cannot be used as a gacha cost. Use a ticket item (e.g. Consumable) instead.");

            if (!item.IsActive)
                throw new ArgumentException($"Item '{item.Name}' is inactive and cannot be used as a gacha cost.");
        }

        // Executes core business logic for get banner by id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed GachaBannerDetailResponseDto? result asynchronously.
        public async Task<GachaBannerDetailResponseDto?> GetBannerById(int id)
        {
            var banner = await _repository.GetGachaBannerByIdWithItems(id);
            if (banner == null)  // Entity not found — short-circuit with appropriate error result
                return null;

            return _mapper.Map<GachaBannerDetailResponseDto>(banner);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for create banner.
        // Returns the computed GachaBannerResponseDto result asynchronously.
        public async Task<GachaBannerResponseDto> CreateBanner(CreateGachaBannerRequestDto request)
        {
            await ValidateCostItem(request.PullCost, request.CostItemId);

            var banner = new GachaBanner
            {
                Name = request.Name,
                Type = request.Type,
                PullCost = request.PullCost,
                CostItemId = request.CostItemId,
                PityLimit = request.PityLimit,
                IsActive = request.IsActive,
                StartAt = request.StartAt.ToUniversalTime(),
                EndAt = request.EndAt.ToUniversalTime()
            };
            var created = await _repository.CreateGachaBanner(banner);
            return _mapper.Map<GachaBannerResponseDto>(created);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for update banner.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed GachaBannerResponseDto result asynchronously.
        public async Task<GachaBannerResponseDto> UpdateBanner(int id, UpdateGachaBannerRequestDto request)
        {
            var banner = await _repository.GetGachaBannerById(id)
                ?? throw new KeyNotFoundException($"GachaBanner with id {id} not found.");

            await ValidateCostItem(request.PullCost, request.CostItemId);

            banner.Name = request.Name;
            banner.Type = request.Type;
            banner.PullCost = request.PullCost;
            banner.CostItemId = request.CostItemId;
            banner.PityLimit = request.PityLimit;
            banner.IsActive = request.IsActive;
            banner.StartAt = request.StartAt.ToUniversalTime();
            banner.EndAt = request.EndAt.ToUniversalTime();

            var updated = await _repository.UpdateGachaBanner(banner);
            return _mapper.Map<GachaBannerResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for add banner item.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed GachaBannerItemResponseDto result asynchronously.
        public async Task<GachaBannerItemResponseDto> AddBannerItem(int bannerId, CreateGachaBannerItemRequestDto request)
        {
            var banner = await _repository.GetGachaBannerById(bannerId)
                ?? throw new KeyNotFoundException($"GachaBanner with id {bannerId} not found.");

            var item = new GachaBannerItem
            {
                GachaBannerId = bannerId,
                ItemId = request.ItemId,
                DropRate = request.DropRate,
                IsFeatured = request.IsFeatured
            };

            var created = await _repository.CreateBannerItem(item);

            return _mapper.Map<GachaBannerItemResponseDto>(created);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for get banners paged.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed PagedResultDto<GachaBannerResponseDto result asynchronously.
        public async Task<PagedResultDto<GachaBannerResponseDto>> GetBannersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetBannersPaged(page, pageSize, search, type, isActive, sortBy, sortOrder);

            var dtos = _mapper.Map<List<GachaBannerResponseDto>>(items);  // Transform domain entity into DTO for the API response layer

            return new PagedResultDto<GachaBannerResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for get banner items paged.
        // Logic details: validates numeric boundary constraints; delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws InvalidOperationException, KeyNotFoundException on invalid state or rule violations.
        // Returns the computed PagedResultDto<GachaBannerItemResponseDto result asynchronously.
        public async Task<PagedResultDto<GachaBannerItemResponseDto>> GetBannerItemsPaged(int page, int pageSize)
        {
            var (totalCount, items) = await _repository.GetBannerItemsPaged(page, pageSize);

            var dtos = _mapper.Map<List<GachaBannerItemResponseDto>>(items);  // Transform domain entity into DTO for the API response layer

            return new PagedResultDto<GachaBannerItemResponseDto>(totalCount, dtos);
        }

        // Validate the active banner, player, payment item, pull count, and pity state; spend currency, select rewards by weighted chance, update inventory and history atomically, then return every pull result.
        public async Task<MultiPullResultDto> Pull(int playerProfileId, int bannerId, GachaPullRequestDto request)
        {
            var banner = await _repository.GetGachaBannerByIdWithItems(bannerId)
                ?? throw new KeyNotFoundException("Gacha banner not found.");

            if (!banner.IsActive || DateTime.UtcNow < banner.StartAt || DateTime.UtcNow > banner.EndAt)
                throw new InvalidOperationException("Gacha banner is not active.");  // Unexpected runtime state — propagate to global error handler

            if (banner.BannerItems == null || !banner.BannerItems.Any())
                throw new InvalidOperationException("Gacha banner has no items.");  // Unexpected runtime state — propagate to global error handler

            return await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                var profile = await _playerProfileRepository.GetPlayerProfileById(playerProfileId)
                    ?? throw new KeyNotFoundException("Player profile not found.");

                bool isFree = request.PullCount == 1 && (!profile.LastFreeGachaTime.HasValue || (DateTime.UtcNow - profile.LastFreeGachaTime.Value).TotalHours >= 24);

                decimal totalCost = 0;

                if (!isFree)
                {
                    totalCost = request.PullCount * banner.PullCost;
                    if (totalCost > 0)
                    {
                        if (!banner.CostItemId.HasValue)
                            throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                                "This gacha banner has no ticket item configured. A paid pull requires a gacha ticket; Coin, Gem and Energy cannot be used.");

                        var playerInventory = await _inventoryRepository.GetByPlayerId(playerProfileId);
                        var allCostItems = playerInventory
                            .Where(i => (banner.CostItemId.HasValue && i.ItemId == banner.CostItemId.Value)
                                     || (i.Item != null && i.Item.Name != null && i.Item.Name.Contains("Ticket", StringComparison.OrdinalIgnoreCase)))
                            .OrderBy(i => i.Quantity)
                            .ToList();

                        int totalAvailable = allCostItems.Sum(i => i.Quantity);
                        if (totalAvailable < totalCost)
                            throw new InvalidOperationException("Not enough gacha tickets or cost items.");  // Unexpected runtime state — propagate to global error handler

                        int remainingToDeduct = (int)totalCost;
                        foreach (var costItem in allCostItems)
                        {
                            if (remainingToDeduct <= 0) break;

                            if (costItem.Quantity <= remainingToDeduct)
                            {
                                remainingToDeduct -= costItem.Quantity;
                                await _inventoryRepository.DeleteItem(costItem.InventoryItemId);
                            }
                            else
                            {
                                costItem.Quantity -= remainingToDeduct;
                                remainingToDeduct = 0;
                                await _inventoryRepository.UpdateItem(costItem);
                            }
                        }
                    }
                }
                else
                {
                    profile.LastFreeGachaTime = DateTime.UtcNow;
                }

                await _playerProfileRepository.UpdatePlayerProfile(profile);

                decimal totalRate = banner.BannerItems.Sum(x => x.DropRate);
                if (totalRate <= 0)
                    throw new InvalidOperationException("Total drop rate is zero.");  // Unexpected runtime state — propagate to global error handler

                var rand = new Random();
                var resultItems = new List<GachaPullResultDto>();
                var pullHistoryForBanner = await _repository.GetPullHistoryByPlayerAndBanner(playerProfileId, bannerId);
                var bannerItemsById = banner.BannerItems.ToDictionary(x => x.ItemId, x => x);
                var featuredBannerItem = banner.BannerItems.FirstOrDefault(x => x.IsFeatured);

                if (featuredBannerItem == null)  // Entity not found — short-circuit with appropriate error result
                    throw new InvalidOperationException("Gacha banner has no featured item.");  // Unexpected runtime state — propagate to global error handler

                for (int i = 0; i < request.PullCount; i++)
                {
                    var consecutiveNonFeaturedPulls = 0;
                    foreach (var previousHistory in pullHistoryForBanner)
                    {
                        var previousItem = bannerItemsById.GetValueOrDefault(previousHistory.RewardItemId);
                        if (previousItem?.IsFeatured == true)
                            break;

                        consecutiveNonFeaturedPulls++;
                    }

                    GachaBannerItem? selected = null;
                    if (banner.PityLimit > 0 && consecutiveNonFeaturedPulls >= banner.PityLimit - 1)
                    {
                        selected = featuredBannerItem;
                    }
                    else
                    {
                        decimal roll = (decimal)rand.NextDouble() * totalRate;
                        decimal current = 0;

                        foreach (var item in banner.BannerItems)
                        {
                            current += item.DropRate;
                            if (roll <= current)
                            {
                                selected = item;
                                break;
                            }
                        }

                        if (selected == null) selected = banner.BannerItems.Last();  // Entity not found — short-circuit with appropriate error result
                    }

                    bool isNew = false;
                    string itemName = selected.Item?.Name ?? string.Empty;
                    // Supported item types: Weapon, Armor, Consumable, Material, QuestItem, or Currency; the type controls filtering, stacking, and usage behavior.
                    string itemType = selected.Item?.Type ?? string.Empty;

                    bool isGold = itemName.Equals("Gold", StringComparison.OrdinalIgnoreCase) ||
                                  itemName.StartsWith("Gold", StringComparison.OrdinalIgnoreCase) ||
                                  (itemType.Equals("Currency", StringComparison.OrdinalIgnoreCase) && itemName.Contains("Gold", StringComparison.OrdinalIgnoreCase));

                    bool isGem = itemName.Equals("Gem", StringComparison.OrdinalIgnoreCase) ||
                                 itemName.Equals("Gems", StringComparison.OrdinalIgnoreCase) ||
                                 itemName.StartsWith("Gem", StringComparison.OrdinalIgnoreCase) ||
                                 (itemType.Equals("Currency", StringComparison.OrdinalIgnoreCase) && itemName.Contains("Gem", StringComparison.OrdinalIgnoreCase));

                    if (isGold)
                    {
                        decimal amount = (selected.Item != null && selected.Item.BaseValue > 0) ? selected.Item.BaseValue : 1000m;
                        profile.Gold += amount;
                        await _playerProfileRepository.UpdatePlayerProfile(profile);
                    }
                    else if (isGem)
                    {
                        decimal amount = (selected.Item != null && selected.Item.BaseValue > 0) ? selected.Item.BaseValue : 10m;
                        profile.Gems += amount;
                        await _playerProfileRepository.UpdatePlayerProfile(profile);
                    }
                    else
                    {
                        var existingInv = await _inventoryRepository.GetByPlayerAndItem(playerProfileId, selected.ItemId);
                        isNew = existingInv == null;
                        await _rewardDeliveryService.DeliverItemAsync(playerProfileId, selected.ItemId, 1, "gacha reward");
                    }

                    var historyEntry = new GachaPullHistory
                    {
                        PlayerProfileId = playerProfileId,
                        GachaBannerId = bannerId,
                        RewardItemId = selected.ItemId,
                        PullCount = 1,
                        CostSpent = isFree && i == 0 ? 0 : banner.PullCost,
                        PulledAt = DateTime.UtcNow
                    };

                    await _repository.AddGachaPullHistory(historyEntry);
                    pullHistoryForBanner.Insert(0, historyEntry);

                    bool selectedIsFeatured = bannerItemsById.TryGetValue(selected.ItemId, out var selectedBannerItem) && selectedBannerItem.IsFeatured;
                    int currentPity = 0;
                    if (!selectedIsFeatured)
                    {
                        currentPity = 1;
                        foreach (var previousHistory in pullHistoryForBanner.Skip(1))  // Apply pagination offset — skip already-seen records
                        {
                            var previousItem = bannerItemsById.GetValueOrDefault(previousHistory.RewardItemId);
                            if (previousItem?.IsFeatured == true)
                                break;

                            currentPity++;
                        }
                    }
                    else
                    {
                        currentPity = 0;
                    }

                    resultItems.Add(new GachaPullResultDto
                    {
                        Success = true,
                        PulledItemId = selected.ItemId,
                        PulledItemName = selected.Item?.Name ?? "Unknown",
                        PulledItemIconUrl = selected.Item?.IconUrl,
                        PulledItemRarity = selected.Item?.Rarity ?? "Common",
                        IsNew = isNew,
                        PityCounter = currentPity,
                        CurrentPity = currentPity
                    });
                }

                return new MultiPullResultDto
                {
                    Success = true,
                    Message = "Pulled successfully.",
                    PulledItems = resultItems,
                    TotalCost = totalCost
                };
            });
        }

        // Executes core business logic for get history paged.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed PagedResultDto<GachaPullHistoryResponseDto result asynchronously.
        public async Task<PagedResultDto<GachaPullHistoryResponseDto>> GetHistoryPaged(int playerProfileId, int page, int pageSize)
        {
            var (totalCount, items) = await _repository.GetGachaPullHistoryPaged(playerProfileId, page, pageSize);

            var dtos = items.Select(h => new GachaPullHistoryResponseDto
            {
                GachaPullHistoryId = h.GachaPullHistoryId,
                PlayerProfileId = h.PlayerProfileId,
                GachaBannerId = h.GachaBannerId,
                BannerName = h.GachaBanner?.Name,
                RewardItemId = h.RewardItemId,
                RewardItemName = h.RewardItem?.Name,
                RewardItemIconUrl = h.RewardItem?.IconUrl,
                RewardItemRarity = h.RewardItem?.Rarity,
                PullCount = h.PullCount,
                CostSpent = h.CostSpent,
                PulledAt = h.PulledAt
            }).ToList();

            return new PagedResultDto<GachaPullHistoryResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for remove banner item.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed bool result asynchronously.
        public async Task<bool> RemoveBannerItem(int bannerId, int bannerItemId)
        {
            return await _repository.RemoveBannerItem(bannerId, bannerItemId);
        }

        // Executes core business logic for get all history paged.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed PagedResultDto<GachaPullHistoryResponseDto result asynchronously.
        public async Task<PagedResultDto<GachaPullHistoryResponseDto>> GetAllHistoryPaged(int page, int pageSize, int? bannerId, string? rarity)
        {
            var (totalCount, items) = await _repository.GetAllGachaPullHistoryPaged(page, pageSize, bannerId, rarity);
            var dtos = items.Select(h => new GachaPullHistoryResponseDto
            {
                GachaPullHistoryId = h.GachaPullHistoryId,
                PlayerProfileId = h.PlayerProfileId,
                GachaBannerId = h.GachaBannerId,
                BannerName = h.GachaBanner?.Name,
                RewardItemId = h.RewardItemId,
                RewardItemName = h.RewardItem?.Name,
                RewardItemIconUrl = h.RewardItem?.IconUrl,
                RewardItemRarity = h.RewardItem?.Rarity,
                PullCount = h.PullCount,
                CostSpent = h.CostSpent,
                PulledAt = h.PulledAt
            }).ToList();
            return new PagedResultDto<GachaPullHistoryResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for get player gacha stats.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed PlayerGachaStatsDto? result asynchronously.
        public async Task<PlayerGachaStatsDto?> GetPlayerGachaStats(int playerProfileId)
        {
            var stats = await _repository.GetPlayerGachaStatsAsync(playerProfileId);
            if (stats == null) return null;  // Entity not found — short-circuit with appropriate error result

            decimal actualRate = stats.Value.TotalPulls > 0
                ? ((decimal)stats.Value.LegendaryPulls / stats.Value.TotalPulls) * 100
                : 0;

            return new PlayerGachaStatsDto
            {
                PlayerProfileId = playerProfileId,
                PlayerName = stats.Value.PlayerName,
                AccountId = stats.Value.AccountId,
                TotalPulls = stats.Value.TotalPulls,
                TotalCost = stats.Value.TotalCost,
                LegendaryPulls = stats.Value.LegendaryPulls,
                ActualLegendaryRate = Math.Round(actualRate, 2),
                SystemLegendaryRate = 1.0m
            };
        }
    }
}
