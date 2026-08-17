using AutoMapper;
using AutoMapper.QueryableExtensions;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace BLL.Services
{
    // Executes core business logic for i dungeon config service.
    public class DungeonConfigService : IDungeonConfigService
    {
        private readonly IDungeonConfigRepository _repository;
        private readonly IChestRepository _chestRepository;
        private readonly IMapper _mapper;

        // Initializes a new instance of DungeonConfigService with dependencies: repository, chestRepository, mapper.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public DungeonConfigService(IDungeonConfigRepository repository, IChestRepository chestRepository, IMapper mapper)
        {
            _repository = repository;
            _chestRepository = chestRepository;
            _mapper = mapper;
        }

        // Executes core business logic for get dungeon by id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed DungeonConfigResponseDto? result asynchronously.
        public async Task<DungeonConfigResponseDto?> GetDungeonById(int id)
        {
            var dungeon = await _repository.GetByIdWithChest(id);
            if (dungeon == null)  // Entity not found — short-circuit with appropriate error result
                return null;

            var dto = _mapper.Map<DungeonConfigResponseDto>(dungeon);  // Transform domain entity into DTO for the API response layer
            if (dungeon.Chest != null)
            {
                dto.GoldMinReward = dungeon.Chest.GoldMinReward;
                dto.GoldMaxReward = dungeon.Chest.GoldMaxReward;
                dto.ExperienceReward = dungeon.Chest.ExperienceReward;
                if (dungeon.Chest.ChestItems != null)
                {
                    dto.PossibleDrops = _mapper.Map<List<ChestItemResponseDto>>(dungeon.Chest.ChestItems);  // Transform domain entity into DTO for the API response layer
                }
            }
            return dto;
        }

        // Executes core business logic for update dungeon.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed DungeonConfigResponseDto result asynchronously.
        public async Task<DungeonConfigResponseDto> UpdateDungeon(int id, UpdateDungeonConfigRequestDto request)
        {
            var dungeon = await _repository.GetDungeonConfigById(id)
                ?? throw new KeyNotFoundException($"DungeonConfig with id {id} not found.");

            dungeon.Name = request.Name;
            dungeon.Description = request.Description;
            dungeon.Type = request.Type;
            dungeon.LevelRequirement = request.LevelRequirement;
            dungeon.MaxMembers = request.MaxMembers;
            dungeon.Difficulty = request.Difficulty;
            dungeon.RecommendedPower = request.RecommendedPower;
            dungeon.EnergyCost = request.EnergyCost;
            dungeon.ChestId = request.ChestId;
            dungeon.IsActive = request.IsActive;

            var updated = await _repository.UpdateDungeonConfig(dungeon);
            return _mapper.Map<DungeonConfigResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for get dungeons paged.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed PagedResultDto<DungeonConfigResponseDto result asynchronously.
        public async Task<PagedResultDto<DungeonConfigResponseDto>> GetDungeonsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetDungeonsPaged(page, pageSize, search, type, isActive, sortBy, sortOrder);

            var dtos = _mapper.Map<List<DungeonConfigResponseDto>>(items);  // Transform domain entity into DTO for the API response layer
            for (int i = 0; i < dtos.Count; i++)
            {
                var dungeon = items[i];
                if (dungeon.Chest != null)
                {
                    dtos[i].GoldMinReward = dungeon.Chest.GoldMinReward;
                    dtos[i].GoldMaxReward = dungeon.Chest.GoldMaxReward;
                    dtos[i].ExperienceReward = dungeon.Chest.ExperienceReward;
                    if (dungeon.Chest.ChestItems != null)
                    {
                        dtos[i].PossibleDrops = _mapper.Map<List<ChestItemResponseDto>>(dungeon.Chest.ChestItems);  // Transform domain entity into DTO for the API response layer
                    }
                }
            }
            return new PagedResultDto<DungeonConfigResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for add chest item.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed ChestItemResponseDto result asynchronously.
        public async Task<ChestItemResponseDto> AddChestItem(int dungeonId, CreateChestItemRequestDto request)
        {
            var dungeon = await _repository.GetByIdWithChest(dungeonId);
            if (dungeon == null) throw new KeyNotFoundException("Dungeon not found.");  // Entity not found — short-circuit with appropriate error result

            if (dungeon.ChestId == null)
            {
                var newChest = new Chest { Name = dungeon.Name + " Chest" };
                await _chestRepository.CreateChest(newChest);
                dungeon.ChestId = newChest.ChestId;
                await _repository.UpdateDungeonConfig(dungeon);
            }

            var chestItem = new ChestItem
            {
                ChestId = dungeon.ChestId.Value,
                ItemId = request.ItemId,
                QuantityMin = request.QuantityMin,
                QuantityMax = request.QuantityMax,
                DropRate = request.DropRate,
                IsGuaranteed = request.IsGuaranteed
            };

            await _chestRepository.AddChestItem(chestItem);
            var savedItem = await _chestRepository.GetChestItemById(chestItem.ChestItemId);
            return _mapper.Map<ChestItemResponseDto>(savedItem);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for update chest item.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed ChestItemResponseDto result asynchronously.
        public async Task<ChestItemResponseDto> UpdateChestItem(int dungeonId, int chestItemId, CreateChestItemRequestDto request)
        {
            var chestItem = await _chestRepository.GetChestItemById(chestItemId);
            if (chestItem == null) throw new KeyNotFoundException("ChestItem not found.");  // Entity not found — short-circuit with appropriate error result

            chestItem.QuantityMin = request.QuantityMin;
            chestItem.QuantityMax = request.QuantityMax;
            chestItem.DropRate = request.DropRate;
            chestItem.IsGuaranteed = request.IsGuaranteed;

            await _chestRepository.UpdateChestItem(chestItem);
            return _mapper.Map<ChestItemResponseDto>(chestItem);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for remove chest item.
        // Logic details: delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        public async Task RemoveChestItem(int dungeonId, int chestItemId)
        {
            await _chestRepository.RemoveChestItem(chestItemId);
        }
    }
}
