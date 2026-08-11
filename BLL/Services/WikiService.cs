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
    // Nghiệp vụ codex công khai. Xem IWikiService.
    public class WikiService : IWikiService
    {
        // Trần pageSize cho API công khai: chặn một request vô danh kéo cả bảng.
        // 1000 đủ cho codex hiện tại (client tải một lần rồi lọc phía trình duyệt).
        private const int MaxPageSize = 1000;

        private readonly IWikiRepository _repository;
        private readonly IMapper _mapper;

        public WikiService(IWikiRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // CLASSES
        // ═══════════════════════════════════════════════════════════════════════

        public async Task<IEnumerable<ClassConfigResponseDto>> GetClasses()
        {
            var configs = await _repository.GetClassConfigs();
            return _mapper.Map<IEnumerable<ClassConfigResponseDto>>(configs);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // MONSTERS
        // ═══════════════════════════════════════════════════════════════════════

        public async Task<PagedResultDto<MonsterResponseDto>> GetMonsters(
            int page, int pageSize, string? search, string? type, string? sortBy, string? sortOrder)
        {
            var (totalCount, items) = await _repository.GetMonstersPaged(
                NormalizePage(page), NormalizePageSize(pageSize), search, type, sortBy, sortOrder);

            var dtos = items.Select(m => _mapper.Map<MonsterResponseDto>(m)).ToList();
            return new PagedResultDto<MonsterResponseDto>(totalCount, dtos);
        }

        public async Task<MonsterDetailResponseDto?> GetMonsterById(int id)
        {
            var monster = await _repository.GetMonsterById(id);
            if (monster == null)
                return null;

            var dto = _mapper.Map<MonsterDetailResponseDto>(monster);

            // Chỉ hiện vật phẩm rơi đang bật: một drop đã tắt vẫn nằm trong bảng
            // nhưng không còn rơi trong game, hiện ra sẽ là thông tin sai.
            if (monster.MonsterDrops != null && monster.MonsterDrops.Any())
                dto.MonsterDrops = _mapper.Map<List<MonsterDropResponseDto>>(monster.MonsterDrops.Where(d => d.IsActive));

            return dto;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ITEMS
        // ═══════════════════════════════════════════════════════════════════════

        public async Task<PagedResultDto<ItemResponseDto>> GetItems(
            int page, int pageSize, string? search, string? type, string? rarity, string? sortBy, string? sortOrder)
        {
            var (totalCount, items) = await _repository.GetItemsPaged(
                NormalizePage(page), NormalizePageSize(pageSize), search, type, rarity, sortBy, sortOrder);

            var dtos = items.Select(ToItemDto).ToList();
            return new PagedResultDto<ItemResponseDto>(totalCount, dtos);
        }

        public async Task<ItemResponseDto?> GetItemById(int id)
        {
            var item = await _repository.GetItemById(id);
            return item == null ? null : ToItemDto(item);
        }

        // EquipmentStats nằm ở bảng khác và CritRate/CritDamage lưu dạng số nguyên
        // đã scale, nên phải dàn phẳng + đổi đơn vị bằng tay sau khi map.
        private ItemResponseDto ToItemDto(Item item)
        {
            var dto = _mapper.Map<ItemResponseDto>(item);

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

        // ═══════════════════════════════════════════════════════════════════════
        // SKILLS
        // ═══════════════════════════════════════════════════════════════════════

        public async Task<PagedResultDto<SkillResponseDto>> GetSkills(
            int page, int pageSize, string? search, string? type)
        {
            var (totalCount, items) = await _repository.GetSkillsPaged(
                NormalizePage(page), NormalizePageSize(pageSize), search, type);

            var dtos = items.Select(s => _mapper.Map<SkillResponseDto>(s)).ToList();
            return new PagedResultDto<SkillResponseDto>(totalCount, dtos);
        }

        public async Task<SkillResponseDto?> GetSkillById(int id)
        {
            var skill = await _repository.GetSkillById(id);
            return skill == null ? null : _mapper.Map<SkillResponseDto>(skill);
        }

        // ── Helpers ────────────────────────────────────────────────

        private static int NormalizePage(int page) => page < 1 ? 1 : page;

        private static int NormalizePageSize(int pageSize) =>
            pageSize < 1 ? 1 : (pageSize > MaxPageSize ? MaxPageSize : pageSize);
    }
}
