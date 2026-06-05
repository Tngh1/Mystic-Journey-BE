using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Linq;

namespace BLL.Services
{
    public class GachaBannerService : IGachaBannerService
    {
        private readonly IGachaBannerRepository _repository;
        private readonly IMapper _mapper;

        public GachaBannerService(IGachaBannerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GachaBannerDetailResponseDto?> GetBannerById(int id)
        {
            var banner = await _repository.GetGachaBannerByIdWithItems(id);
            if (banner == null)
                return null;

            var dto = _mapper.Map<GachaBannerDetailResponseDto>(banner);

            if (banner.BannerItems != null && banner.BannerItems.Any())
            {
                dto.BannerItems = banner.BannerItems.Select(bi => new GachaBannerItemResponseDto
                {
                    Id = bi.GachaBannerItemId,
                    ItemId = bi.ItemId,
                    ItemName = bi.Item?.Name,
                    ItemIconUrl = bi.Item?.IconUrl,
                    ItemRarity = bi.Item?.Rarity,
                    DropRate = bi.DropRate,
                    IsFeatured = bi.IsFeatured
                }).ToList();
            }

            return dto;
        }

        public async Task<GachaBannerResponseDto> CreateBanner(CreateGachaBannerRequestDto request)
        {
            var banner = _mapper.Map<GachaBanner>(request);

            var created = await _repository.CreateGachaBanner(banner);
            return _mapper.Map<GachaBannerResponseDto>(created);
        }

        public async Task<GachaBannerResponseDto> UpdateBanner(int id, UpdateGachaBannerRequestDto request)
        {
            var banner = await _repository.GetGachaBannerById(id)
                ?? throw new KeyNotFoundException($"GachaBanner with id {id} not found.");

            banner.Name = request.Name;
            banner.Type = request.Type;
            banner.PullCost = request.PullCost;
            banner.PityLimit = request.PityLimit;
            banner.IsActive = request.IsActive;
            banner.StartAt = request.StartAt;
            banner.EndAt = request.EndAt;

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

            return new GachaBannerItemResponseDto
            {
                Id = created.GachaBannerItemId,
                ItemId = created.ItemId,
                DropRate = created.DropRate,
                IsFeatured = created.IsFeatured
            };
        }

        public IQueryable<GachaBannerResponseDto> GetBannersQueryable()
        {
            return _repository.GetGachaBannersQueryable()
                .Select(b => new GachaBannerResponseDto
                {
                    Id = b.GachaBannerId,
                    Name = b.Name,
                    Type = b.Type,
                    PullCost = b.PullCost,
                    PityLimit = b.PityLimit,
                    IsActive = b.IsActive,
                    StartAt = b.StartAt,
                    EndAt = b.EndAt
                })
                .AsQueryable();
        }

        public IQueryable<GachaBannerItemResponseDto> GetBannerItemsQueryable()
        {
            return _repository.GetBannerItemsQueryable()
                .Select(bi => new GachaBannerItemResponseDto
                {
                    Id = bi.GachaBannerItemId,
                    ItemId = bi.ItemId,
                    ItemName = bi.Item == null ? null : bi.Item.Name,
                    ItemIconUrl = bi.Item == null ? null : bi.Item.IconUrl,
                    ItemRarity = bi.Item == null ? null : bi.Item.Rarity,
                    DropRate = bi.DropRate,
                    IsFeatured = bi.IsFeatured
                })
                .AsQueryable();
        }
    }
}
