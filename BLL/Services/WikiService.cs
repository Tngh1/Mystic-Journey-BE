using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using BLL.Utils;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    // Executes core business logic for i wiki service.
    public class WikiService : IWikiService
    {
        private const int MaxPageSize = 1000;

        private readonly IWikiRepository _repository;
        private readonly IMapper _mapper;

        // Initializes a new instance of WikiService with dependencies: repository, mapper.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public WikiService(IWikiRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        // Executes core business logic for get classes.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed IEnumerable<ClassConfigResponseDto result asynchronously.
        public async Task<IEnumerable<ClassConfigResponseDto>> GetClasses()
        {
            var configs = await _repository.GetClassConfigs();
            return _mapper.Map<IEnumerable<ClassConfigResponseDto>>(configs);  // Transform domain entity into DTO for the API response layer
        }


        // Load monsters using page, page size, search, and type; it loads monsters paged, projects records into the output shape, builds map, and materializes the query results.
        public async Task<PagedResultDto<MonsterResponseDto>> GetMonsters(
            int page, int pageSize, string? search, string? type, string? sortBy, string? sortOrder)
        {
            var (totalCount, items) = await _repository.GetMonstersPaged(
                NormalizePage(page), NormalizePageSize(pageSize), search, type, sortBy, sortOrder);

            var dtos = items.Select(m => _mapper.Map<MonsterResponseDto>(m)).ToList();  // Transform domain entity into DTO for the API response layer
            return new PagedResultDto<MonsterResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for get monster by id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed MonsterDetailResponseDto? result asynchronously.
        public async Task<MonsterDetailResponseDto?> GetMonsterById(int id)
        {
            var monster = await _repository.GetMonsterById(id);
            if (monster == null)  // Entity not found — short-circuit with appropriate error result
                return null;

            var dto = _mapper.Map<MonsterDetailResponseDto>(monster);  // Transform domain entity into DTO for the API response layer

            if (monster.MonsterDrops != null && monster.MonsterDrops.Any())
                dto.MonsterDrops = _mapper.Map<List<MonsterDropResponseDto>>(monster.MonsterDrops.Where(d => d.IsActive));  // Filter records matching the predicate

            return dto;
        }


        // Load items using page, page size, search, and type; it loads items paged, projects records into the output shape, and materializes the query results.
        public async Task<PagedResultDto<ItemResponseDto>> GetItems(
            int page, int pageSize, string? search, string? type, string? rarity, string? sortBy, string? sortOrder)
        {
            var (totalCount, items) = await _repository.GetItemsPaged(
                NormalizePage(page), NormalizePageSize(pageSize), search, type, rarity, sortBy, sortOrder);

            var dtos = items.Select(ToItemDto).ToList();
            return new PagedResultDto<ItemResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for get item by id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed ItemResponseDto? result asynchronously.
        public async Task<ItemResponseDto?> GetItemById(int id)
        {
            var item = await _repository.GetItemById(id);
            return item == null ? null : ToItemDto(item);
        }

        // Executes core business logic for to item dto.
        // Logic details: transforms domain entities into DTO transfer models.
        private ItemResponseDto ToItemDto(Item item)
        {
            var dto = _mapper.Map<ItemResponseDto>(item);  // Transform domain entity into DTO for the API response layer

            if (item.EquipmentStats != null)
            {
                dto.BaseHp = item.EquipmentStats.BaseHp;
                dto.BaseAtk = item.EquipmentStats.BaseAtk;
                dto.BaseDef = item.EquipmentStats.BaseDef;
                dto.BonusHp = item.EquipmentStats.BonusHp;
                dto.BonusAtk = item.EquipmentStats.BonusAtk;
                dto.BonusDef = item.EquipmentStats.BonusDef;
                dto.BonusCritRate = item.EquipmentStats.BonusCritRate != 0
                    ? StatHelper.FromScaled(item.EquipmentStats.BonusCritRate, StatScale.CritRate) : 0f;
                dto.BonusCritDamage = item.EquipmentStats.BonusCritDamage != 0
                    ? StatHelper.FromScaled(item.EquipmentStats.BonusCritDamage, StatScale.CritRate) : 0f;
            }

            return dto;
        }


        // Load skills using page, page size, search, and type; it loads skills paged, projects records into the output shape, builds map, and materializes the query results.
        public async Task<PagedResultDto<SkillResponseDto>> GetSkills(
            int page, int pageSize, string? search, string? type)
        {
            var (totalCount, items) = await _repository.GetSkillsPaged(
                NormalizePage(page), NormalizePageSize(pageSize), search, type);

            var dtos = items.Select(s => _mapper.Map<SkillResponseDto>(s)).ToList();  // Transform domain entity into DTO for the API response layer
            return new PagedResultDto<SkillResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for get skill by id.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed SkillResponseDto? result asynchronously.
        public async Task<SkillResponseDto?> GetSkillById(int id)
        {
            var skill = await _repository.GetSkillById(id);
            return skill == null ? null : _mapper.Map<SkillResponseDto>(skill);  // Transform domain entity into DTO for the API response layer
        }


        // Executes core business logic for normalize page.
        private static int NormalizePage(int page) => page < 1 ? 1 : page;

        // Executes core business logic for normalize page size.
        private static int NormalizePageSize(int pageSize) =>
            pageSize < 1 ? 1 : (pageSize > MaxPageSize ? MaxPageSize : pageSize);
    }
}
