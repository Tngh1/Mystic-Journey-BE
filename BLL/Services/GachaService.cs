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
    public class GachaService : IGachaService
    {
        private readonly IGachaBannerRepository _bannerRepository;
        private readonly IGachaPullHistoryRepository _pullHistoryRepository;
        private readonly IPlayerProfileRepository _profileRepository;
        private readonly IInventoryService _inventoryService;
        private readonly IPlayerProfileService _playerProfileService;
        private readonly Random _random = new();

        public GachaService(
            IGachaBannerRepository bannerRepository,
            IGachaPullHistoryRepository pullHistoryRepository,
            IPlayerProfileRepository profileRepository,
            IInventoryService inventoryService,
            IPlayerProfileService playerProfileService)
        {
            _bannerRepository = bannerRepository;
            _pullHistoryRepository = pullHistoryRepository;
            _profileRepository = profileRepository;
            _inventoryService = inventoryService;
            _playerProfileService = playerProfileService;
        }

        public async Task<GachaBannerListResponseDto> GetAllBannersAsync()
        {
            var banners = await _bannerRepository.GetAllActiveAsync();

            var dtos = banners.Select(b => new GachaBannerResponseDto
            {
                BannerId = b.Id,
                Name = b.Name,
                Type = b.Type.ToString(),
                PullCost = b.PullCost,
                PityLimit = b.PityLimit,
                IsActive = b.IsActive,
                StartAt = b.StartAt,
                EndAt = b.EndAt
            }).ToList();

            return new GachaBannerListResponseDto
            {
                Success = true,
                Message = "Banners retrieved successfully.",
                Banners = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<GachaBannerListResponseDto> GetAvailableBannersAsync()
        {
            var banners = await _bannerRepository.GetAvailableNowAsync();

            var dtos = banners.Select(b => new GachaBannerResponseDto
            {
                BannerId = b.Id,
                Name = b.Name,
                Type = b.Type.ToString(),
                PullCost = b.PullCost,
                PityLimit = b.PityLimit,
                IsActive = b.IsActive,
                StartAt = b.StartAt,
                EndAt = b.EndAt
            }).ToList();

            return new GachaBannerListResponseDto
            {
                Success = true,
                Message = "Available banners retrieved successfully.",
                Banners = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<GachaApiResponseDto> GetBannerByIdAsync(Guid bannerId)
        {
            var banner = await _bannerRepository.GetByIdAsync(bannerId);

            if (banner == null)
            {
                return new GachaApiResponseDto
                {
                    Success = false,
                    Message = "Banner not found."
                };
            }

            var dto = new GachaBannerResponseDto
            {
                BannerId = banner.Id,
                Name = banner.Name,
                Type = banner.Type.ToString(),
                PullCost = banner.PullCost,
                PityLimit = banner.PityLimit,
                IsActive = banner.IsActive,
                StartAt = banner.StartAt,
                EndAt = banner.EndAt
            };

            return new GachaApiResponseDto
            {
                Success = true,
                Message = "Banner retrieved successfully.",
                Banner = dto
            };
        }

        public async Task<GachaApiResponseDto> PullGachaAsync(Guid accountId, GachaPullRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new GachaApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var banner = await _bannerRepository.GetByIdWithItemsAsync(request.BannerId);
            if (banner == null || !banner.IsActive)
            {
                return new GachaApiResponseDto
                {
                    Success = false,
                    Message = "Banner not found or not active."
                };
            }

            var now = DateTime.UtcNow;
            if (banner.StartAt > now || banner.EndAt < now)
            {
                return new GachaApiResponseDto
                {
                    Success = false,
                    Message = "This banner is not currently available."
                };
            }

            var pullCount = Math.Clamp(request.PullCount, 1, 10);
            var totalCost = banner.PullCost * pullCount;

            if (profile.Gems < totalCost)
            {
                return new GachaApiResponseDto
                {
                    Success = false,
                    Message = $"Not enough gems. Required: {totalCost}, Available: {profile.Gems}"
                };
            }

            profile.Gems -= totalCost;
            await _profileRepository.UpdateCurrencyAsync(profile.Id, null, profile.Gems, null);

            var pullResults = new List<GachaPullResultDto>();
            var pullsSinceLastFeatured = await _pullHistoryRepository.GetPullCountSinceLastFeaturedAsync(profile.Id, banner.Id);

            for (int i = 0; i < pullCount; i++)
            {
                pullsSinceLastFeatured++;
                var (item, isPity) = SelectRandomItem(banner, pullsSinceLastFeatured);

                if (isPity)
                {
                    pullsSinceLastFeatured = 0;
                }

                var pullHistory = new GachaPullHistory
                {
                    Id = Guid.NewGuid(),
                    PlayerProfileId = profile.Id,
                    GachaBannerId = banner.Id,
                    RewardItemId = item.Id,
                    PullCount = i + 1,
                    CostSpent = banner.PullCost,
                    PulledAt = DateTime.UtcNow
                };

                await _pullHistoryRepository.CreateAsync(pullHistory);

                await _inventoryService.AddItemToInventoryAsync(accountId, new AddItemToInventoryRequestDto
                {
                    ItemId = item.Id,
                    Quantity = 1
                });

                var featuredItem = banner.BannerItems.FirstOrDefault(bi => bi.ItemId == item.Id && bi.IsFeatured);

                pullResults.Add(new GachaPullResultDto
                {
                    RewardItemId = item.Id,
                    ItemName = item.Name,
                    ItemRarity = item.Rarity.ToString(),
                    IconUrl = item.IconUrl,
                    IsFeatured = featuredItem?.IsFeatured ?? false,
                    PullNumber = i + 1
                });
            }

            var currencyResponse = new PlayerCurrencyResponseDto
            {
                Gold = profile.Gold,
                Gems = profile.Gems,
                Energy = profile.Energy,
                MaxEnergy = 100
            };

            return new GachaApiResponseDto
            {
                Success = true,
                Message = $"You pulled {pullCount} time(s)!",
                PullResults = pullResults,
                Currency = currencyResponse
            };
        }

        private (Item item, bool isPity) SelectRandomItem(GachaBanner banner, int pullsSinceLastFeatured)
        {
            if (banner.BannerItems == null || !banner.BannerItems.Any())
            {
                return (new Item { Id = Guid.Empty, Name = "Empty", Rarity = Item.ItemRarity.Common }, false);
            }

            double pityBonus = 0;
            if (pullsSinceLastFeatured >= banner.PityLimit - 10)
            {
                pityBonus = (pullsSinceLastFeatured - (banner.PityLimit - 10)) * 0.5;
            }

            var totalRate = banner.BannerItems.Sum(bi => (double)bi.DropRate);
            var roll = _random.NextDouble() * totalRate;

            if (pityBonus > 0 && _random.NextDouble() < pityBonus / 100)
            {
                var featuredItems = banner.BannerItems.Where(bi => bi.IsFeatured).ToList();
                if (featuredItems.Any())
                {
                    var featuredItem = featuredItems[_random.Next(featuredItems.Count)];
                    var item = featuredItem.Item!;
                    return (item, true);
                }
            }

            double cumulative = 0;
            foreach (var bannerItem in banner.BannerItems)
            {
                cumulative += (double)bannerItem.DropRate;
                if (roll <= cumulative)
                {
                    return (bannerItem.Item!, false);
                }
            }

            return (banner.BannerItems.First().Item!, false);
        }

        public async Task<GachaHistoryListResponseDto> GetPullHistoryAsync(Guid accountId, int pageNumber = 1, int pageSize = 20)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new GachaHistoryListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var history = await _pullHistoryRepository.GetByPlayerProfileIdAsync(profile.Id, pageNumber, pageSize);
            var totalCount = await _pullHistoryRepository.GetTotalCountAsync(profile.Id);

            var dtos = history.Select(h => new GachaPullHistoryResponseDto
            {
                PullId = h.Id,
                PlayerProfileId = h.PlayerProfileId,
                BannerId = h.GachaBannerId,
                BannerName = h.GachaBanner?.Name ?? string.Empty,
                RewardItemId = h.RewardItemId,
                RewardItemName = h.RewardItem?.Name ?? string.Empty,
                RewardItemRarity = h.RewardItem?.Rarity.ToString() ?? string.Empty,
                PullCount = h.PullCount,
                CostSpent = h.CostSpent,
                PulledAt = h.PulledAt
            }).ToList();

            return new GachaHistoryListResponseDto
            {
                Success = true,
                Message = "Pull history retrieved successfully.",
                History = dtos,
                TotalCount = totalCount
            };
        }
    }
}
