using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<Quest?> GetByIdAsync(Guid questId)
        {
            return await _context.Quests
                .FirstOrDefaultAsync(q => q.Id == questId && q.IsActive);
        }

        public async Task<Quest?> GetByIdWithRewardAsync(Guid questId)
        {
            return await _context.Quests
                .Include(q => q.RewardItem)
                .FirstOrDefaultAsync(q => q.Id == questId && q.IsActive);
        }

        public async Task<List<Quest>> GetAllActiveAsync()
        {
            return await _context.Quests
                .Include(q => q.RewardItem)
                .Where(q => q.IsActive)
                .OrderBy(q => q.RequiredLevel)
                .ToListAsync();
        }

        public async Task<List<Quest>> GetByTypeAsync(Quest.QuestType type)
        {
            return await _context.Quests
                .Include(q => q.RewardItem)
                .Where(q => q.Type == type && q.IsActive)
                .OrderBy(q => q.RequiredLevel)
                .ToListAsync();
        }

        public async Task<List<Quest>> GetAvailableForLevelAsync(int playerLevel)
        {
            return await _context.Quests
                .Include(q => q.RewardItem)
                .Where(q => q.RequiredLevel <= playerLevel && q.IsActive)
                .OrderBy(q => q.RequiredLevel)
                .ToListAsync();
        }

        public async Task<Quest> CreateAsync(Quest quest)
        {
            await _context.Quests.AddAsync(quest);
            await _context.SaveChangesAsync();
            return quest;
        }

        public async Task<Quest> UpdateAsync(Quest quest)
        {
            _context.Quests.Update(quest);
            await _context.SaveChangesAsync();
            return quest;
        }
    }
}
