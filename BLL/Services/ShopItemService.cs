using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services
{
    // Executes core business logic for i shop item service.
    public class ShopItemService : IShopItemService
    {
        private readonly IShopItemRepository _repository;
        private readonly IMapper _mapper;

        // Initializes a new instance of ShopItemService with dependencies: repository, mapper.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ShopItemService(IShopItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Executes core business logic for get shop item by id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed ShopItemResponseDto? result asynchronously.
        public async Task<ShopItemResponseDto?> GetShopItemById(int id)
        {
            var shopItem = await _repository.GetShopItemByIdWithItem(id);
            if (shopItem == null)  // Entity not found — short-circuit with appropriate error result
                return null;

            return _mapper.Map<ShopItemResponseDto>(shopItem);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for create shop item.
        // Returns the computed ShopItemResponseDto result asynchronously.
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
            return createdDto ?? _mapper.Map<ShopItemResponseDto>(created);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for update shop item.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed ShopItemResponseDto result asynchronously.
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
            return _mapper.Map<ShopItemResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
        }

        // Load shop items paged using page, page size, search, and currency; it builds map.
        public async Task<PagedResultDto<ShopItemResponseDto>> GetShopItemsPaged(
            int page,
            int pageSize,
            string? search,
            // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
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

            var dtos = _mapper.Map<List<ShopItemResponseDto>>(items);  // Transform domain entity into DTO for the API response layer
            return new PagedResultDto<ShopItemResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for normalize shop section.
        // Logic details: validates required non-empty string arguments; throws BadRequestException on invalid state or rule violations.
        private static string NormalizeShopSection(string? shopSection)
            => NormalizeOptionalShopSection(shopSection) ?? ShopSections.Fixed;

        // Executes core business logic for normalize optional shop section.
        // Logic details: validates required non-empty string arguments.
        private static string? NormalizeOptionalShopSection(string? shopSection)
        {
            if (string.IsNullOrWhiteSpace(shopSection))  // Mandatory string argument is blank — fail fast
                return null;

            if (string.Equals(shopSection, ShopSections.Fixed, StringComparison.OrdinalIgnoreCase))
                return ShopSections.Fixed;

            if (string.Equals(shopSection, ShopSections.DailyDeal, StringComparison.OrdinalIgnoreCase))
                return ShopSections.DailyDeal;

            throw new BadRequestException("Shop section must be Fixed or DailyDeal.");  // Business rule violation — surface as 400 Bad Request
        }
    }
}
