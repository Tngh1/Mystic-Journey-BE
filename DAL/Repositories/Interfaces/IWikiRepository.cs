using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IWikiRepository class.
    public interface IWikiRepository
    {
        Task<List<ClassConfig>> GetClassConfigs();

        Task<(int TotalCount, List<Monster> Items)> GetMonstersPaged(
            int page, int pageSize, string? search, string? type, string? sortBy, string? sortOrder);

        Task<Monster?> GetMonsterById(int id);

        Task<(int TotalCount, List<Item> Items)> GetItemsPaged(
            int page, int pageSize, string? search, string? type, string? rarity, string? sortBy, string? sortOrder);

        Task<Item?> GetItemById(int id);

        Task<(int TotalCount, List<Skill> Items)> GetSkillsPaged(
            int page, int pageSize, string? search, string? type);

        Task<Skill?> GetSkillById(int id);
    }
}
