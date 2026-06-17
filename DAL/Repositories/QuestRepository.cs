using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class QuestRepository : IQuestRepository
    {
        private readonly MysticJourneyDbContext _context;

        public QuestRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Quest?> GetQuestById(int id)
        {
            return await _context.Quests
                .FirstOrDefaultAsync(q => q.QuestId == id);
        }

        public async Task<Quest?> GetByIdWithReward(int id)
        {
            return await _context.Quests
                .Include(q => q.RewardItem)
                .FirstOrDefaultAsync(q => q.QuestId == id);
        }

        public async Task<List<Quest>> GetAllQuests()
        {
            return await _context.Quests.ToListAsync();
        }

        public async Task<List<Quest>> GetActiveQuests()
        {
            return await _context.Quests
                .Include(q => q.RewardItem)
                .Where(q => q.IsActive)
                .ToListAsync();
        }

        public async Task<Quest> CreateQuest(Quest quest)
        {
            await _context.Quests.AddAsync(quest);
            await _context.SaveChangesAsync();
            return quest;
        }

        public async Task<Quest> UpdateQuest(Quest quest)
        {
            _context.Quests.Update(quest);
            await _context.SaveChangesAsync();
            return quest;
        }


        public async Task<(int TotalCount, List<Quest> Items)> GetQuestsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? mapName)
        {
            var query = _context.Quests
                .Include(q => q.RewardItem)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Title.Contains(search));
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x => x.Type == type);
            }
            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }
            if (!string.IsNullOrEmpty(mapName))
            {
                query = query.Where(x => x.MapName == mapName);
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
