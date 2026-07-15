using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services
{
    public class ShopItemService : IShopItemService
    {
        private readonly IShopItemRepository _repository;
        private readonly IMapper _mapper;

        public ShopItemService(IShopItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ShopItemResponseDto?> GetShopItemById(int id)
        {
            var shopItem = await _repository.GetShopItemByIdWithItem(id);
            if (shopItem == null)
                return null;

            return _mapper.Map<ShopItemResponseDto>(shopItem);
        }

        public async Task<ShopItemResponseDto> CreateShopItem(CreateShopItemRequestDto request)
        {
            var shopItem = new ShopItem
            {
                ItemId = request.ItemId,
                ShopSection = NormalizeShopSection(request.ShopSection),
                Currency = request.Currency,
                Price = request.Price,
                Stock = request.Stock,
                DailyPurchaseLimit = request.DailyPurchaseLimit,
                WeeklyPurchaseLimit = request.WeeklyPurchaseLimit,
                IsActive = request.IsActive,
                AvailableFrom = request.AvailableFrom,
                AvailableTo = request.AvailableTo
            };

            var created = await _repository.CreateShopItem(shopItem);
            var createdDto = await GetShopItemById(created.ShopItemId);
            return createdDto ?? _mapper.Map<ShopItemResponseDto>(created);
        }

        public async Task<ShopItemResponseDto> UpdateShopItem(int id, UpdateShopItemRequestDto request)
        {
            var shopItem = await _repository.GetShopItemByIdWithItem(id)
                ?? throw new KeyNotFoundException($"ShopItem with id {id} not found.");

            shopItem.ItemId = request.ItemId;
            shopItem.ShopSection = NormalizeShopSection(request.ShopSection);
            shopItem.Currency = request.Currency;
            shopItem.Price = request.Price;
            shopItem.Stock = request.Stock;
            shopItem.DailyPurchaseLimit = request.DailyPurchaseLimit;
            shopItem.WeeklyPurchaseLimit = request.WeeklyPurchaseLimit;
            shopItem.IsActive = request.IsActive;
            shopItem.AvailableFrom = request.AvailableFrom;
            shopItem.AvailableTo = request.AvailableTo;

            var updated = await _repository.UpdateShopItem(shopItem);
            return _mapper.Map<ShopItemResponseDto>(updated);
        }

        public async Task<PagedResultDto<ShopItemResponseDto>> GetShopItemsPaged(
            int page,
            int pageSize,
            string? search,
            string? currency,
            string? shopSection,
            bool? isActive,
            string? sortBy = null,
            string? sortOrder = null)
        {
            var normalizedSection = NormalizeOptionalShopSection(shopSection);
            var (totalCount, items) = await _repository.GetShopItemsPaged(
                page,
                pageSize,
                search,
                currency,
                normalizedSection,
                isActive,
                sortBy,
                sortOrder);

            var dtos = _mapper.Map<List<ShopItemResponseDto>>(items);
            return new PagedResultDto<ShopItemResponseDto>(totalCount, dtos);
        }

        private static string NormalizeShopSection(string? shopSection)
            => NormalizeOptionalShopSection(shopSection) ?? ShopSections.Fixed;

        private static string? NormalizeOptionalShopSection(string? shopSection)
        {
            if (string.IsNullOrWhiteSpace(shopSection))
                return null;

            if (string.Equals(shopSection, ShopSections.Fixed, StringComparison.OrdinalIgnoreCase))
                return ShopSections.Fixed;

            if (string.Equals(shopSection, ShopSections.DailyDeal, StringComparison.OrdinalIgnoreCase))
                return ShopSections.DailyDeal;

            throw new BadRequestException("Shop section must be Fixed or DailyDeal.");
        }
    }
}