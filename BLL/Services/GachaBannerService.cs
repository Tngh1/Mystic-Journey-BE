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
    public class GachaBannerService : IGachaBannerService
    {
        private readonly IGachaBannerRepository _repository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly IMapper _mapper;

        public GachaBannerService(
            IGachaBannerRepository repository,
            IPlayerProfileRepository playerProfileRepository,
            IInventoryRepository inventoryRepository,
            IItemRepository itemRepository,
            ITransactionManager transactionManager,
            IMapper mapper)
        {
            _repository = repository;
            _playerProfileRepository = playerProfileRepository;
            _inventoryRepository = inventoryRepository;
            _itemRepository = itemRepository;
            _transactionManager = transactionManager;
            _mapper = mapper;
        }

        // BR-053 / BR-136: gacha chi nhan gacha ticket item.
        // Chan 2 kieu cau hinh sai:
        //   1. PullCost > 0 nhung khong co CostItemId -> pull se khong the tru gi.
        //   2. CostItemId tro vao item Type = "Currency" (Gold / Gem / Exp)
        //      -> lach BR bang cau hinh thay vi bang code.
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

        public async Task<GachaBannerDetailResponseDto?> GetBannerById(int id)
        {
            var banner = await _repository.GetGachaBannerByIdWithItems(id);
            if (banner == null)
                return null;

            return _mapper.Map<GachaBannerDetailResponseDto>(banner);
        }

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
            return _mapper.Map<GachaBannerResponseDto>(created);
        }

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
            return _mapper.Map<GachaBannerResponseDto>(updated);
        }

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

            return _mapper.Map<GachaBannerItemResponseDto>(created);
        }

        public async Task<PagedResultDto<GachaBannerResponseDto>> GetBannersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetBannersPaged(page, pageSize, search, type, isActive, sortBy, sortOrder);

            var dtos = _mapper.Map<List<GachaBannerResponseDto>>(items);

            return new PagedResultDto<GachaBannerResponseDto>(totalCount, dtos);
        }

        public async Task<PagedResultDto<GachaBannerItemResponseDto>> GetBannerItemsPaged(int page, int pageSize)
        {
            var (totalCount, items) = await _repository.GetBannerItemsPaged(page, pageSize);

            var dtos = _mapper.Map<List<GachaBannerItemResponseDto>>(items);

            return new PagedResultDto<GachaBannerItemResponseDto>(totalCount, dtos);
        }

        public async Task<MultiPullResultDto> Pull(int playerProfileId, int bannerId, GachaPullRequestDto request)
        {
            var banner = await _repository.GetGachaBannerByIdWithItems(bannerId)
                ?? throw new KeyNotFoundException("Gacha banner not found.");

            if (!banner.IsActive || DateTime.UtcNow < banner.StartAt || DateTime.UtcNow > banner.EndAt)
                throw new InvalidOperationException("Gacha banner is not active.");

            if (banner.BannerItems == null || !banner.BannerItems.Any())
                throw new InvalidOperationException("Gacha banner has no items.");

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
                        // BR-053 / BR-136: gacha CHI nhan gacha ticket item.
                        // Coin (Gold), Gem va Energy tuyet doi khong duoc dung de pull.
                        // Truoc day khi banner khong cau hinh CostItemId thi code fallback
                        // sang tru profile.Gems -> vi pham BR. Nay reject thang.
                        if (!banner.CostItemId.HasValue)
                            throw new InvalidOperationException(
                                "This gacha banner has no ticket item configured. A paid pull requires a gacha ticket; Coin, Gem and Energy cannot be used.");

                        var invCostItem = await _inventoryRepository.GetByPlayerAndItem(playerProfileId, banner.CostItemId.Value);
                        if (invCostItem == null || invCostItem.Quantity < totalCost)
                            throw new InvalidOperationException("Not enough gacha tickets or cost items.");

                        invCostItem.Quantity -= (int)totalCost;
                        if (invCostItem.Quantity <= 0)
                            await _inventoryRepository.DeleteItem(invCostItem.InventoryItemId);
                        else
                            await _inventoryRepository.UpdateItem(invCostItem);
                    }
                }
                else
                {
                    profile.LastFreeGachaTime = DateTime.UtcNow;
                }

                await _playerProfileRepository.UpdatePlayerProfile(profile);

                decimal totalRate = banner.BannerItems.Sum(x => x.DropRate);
                if (totalRate <= 0)
                    throw new InvalidOperationException("Total drop rate is zero.");

                var rand = new Random();
                var resultItems = new List<GachaPullResultDto>();
                var pullHistoryForBanner = await _repository.GetPullHistoryByPlayerAndBanner(playerProfileId, bannerId);
                var bannerItemsById = banner.BannerItems.ToDictionary(x => x.ItemId, x => x);
                var featuredBannerItem = banner.BannerItems.FirstOrDefault(x => x.IsFeatured);

                if (featuredBannerItem == null)
                    throw new InvalidOperationException("Gacha banner has no featured item.");

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

                        if (selected == null) selected = banner.BannerItems.Last();
                    }

                    // Check if it's "Gold" or normal item
                    bool isNew = false;
                    if (selected.Item != null && selected.Item.Name.Equals("Gold", StringComparison.OrdinalIgnoreCase))
                    {
                        profile.Gold += selected.Item.BaseValue > 0 ? selected.Item.BaseValue : 1000;
                        await _playerProfileRepository.UpdatePlayerProfile(profile);
                    }
                    else
                    {
                        var existingInv = await _inventoryRepository.GetByPlayerAndItem(playerProfileId, selected.ItemId);
                        if (existingInv != null)
                        {
                            existingInv.Quantity += 1;
                            await _inventoryRepository.UpdateItem(existingInv);
                        }
                        else
                        {
                            isNew = true;
                            await _inventoryRepository.AddItem(new InventoryItem
                            {
                                PlayerProfileId = playerProfileId,
                                ItemId = selected.ItemId,
                                Quantity = 1,
                                IsEquipped = false,
                                IsSkin = false,
                                EnhancementLevel = 0
                            });
                        }
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
                        foreach (var previousHistory in pullHistoryForBanner.Skip(1))
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

        public async Task<bool> RemoveBannerItem(int bannerId, int bannerItemId)
        {
            return await _repository.RemoveBannerItem(bannerId, bannerItemId);
        }

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

        public async Task<PlayerGachaStatsDto?> GetPlayerGachaStats(int playerProfileId)
        {
            var stats = await _repository.GetPlayerGachaStatsAsync(playerProfileId);
            if (stats == null) return null;

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
