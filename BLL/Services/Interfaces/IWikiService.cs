using BLL.DTOs;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IWikiService class.
    public interface IWikiService
    {
        Task<IEnumerable<ClassConfigResponseDto>> GetClasses();

        Task<PagedResultDto<MonsterResponseDto>> GetMonsters(
            int page, int pageSize, string? search, string? type, string? sortBy, string? sortOrder);

        Task<MonsterDetailResponseDto?> GetMonsterById(int id);

        Task<PagedResultDto<ItemResponseDto>> GetItems(
            int page, int pageSize, string? search, string? type, string? rarity, string? sortBy, string? sortOrder);

        Task<ItemResponseDto?> GetItemById(int id);

        Task<PagedResultDto<SkillResponseDto>> GetSkills(
            int page, int pageSize, string? search, string? type);

        Task<SkillResponseDto?> GetSkillById(int id);
    }
}
