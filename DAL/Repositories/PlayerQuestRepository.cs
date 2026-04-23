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
    public class PlayerQuestRepository : IPlayerQuestRepository
    {
        private readonly MysticJourneyDbContext _context;

        public PlayerQuestRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerQuest?> GetByIdAsync(Guid playerQuestId)
        {
            return await _context.PlayerQuests
                .FirstOrDefaultAsync(pq => pq.Id == playerQuestId);
        }

        public async Task<PlayerQuest?> GetByIdWithDetailsAsync(Guid playerQuestId)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItem)
                .Include(pq => pq.PlayerProfile)
                .FirstOrDefaultAsync(pq => pq.Id == playerQuestId);
        }

        public async Task<List<PlayerQuest>> GetByPlayerProfileIdAsync(Guid playerProfileId)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItem)
                .Where(pq => pq.PlayerProfileId == playerProfileId)
                .OrderByDescending(pq => pq.AcceptedAt)
                .ToListAsync();
        }

        public async Task<List<PlayerQuest>> GetActiveQuestsAsync(Guid playerProfileId)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItem)
                .Where(pq => pq.PlayerProfileId == playerProfileId &&
                            (pq.Status == Quest.QuestStatus.InProgress || pq.Status == Quest.QuestStatus.NotStarted))
                .OrderBy(q => q.Quest!.RequiredLevel)
                .ToListAsync();
        }

        public async Task<List<PlayerQuest>> GetCompletedQuestsAsync(Guid playerProfileId)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItem)
                .Where(pq => pq.PlayerProfileId == playerProfileId &&
                            (pq.Status == Quest.QuestStatus.Completed || pq.Status == Quest.QuestStatus.Claimed))
                .OrderByDescending(pq => pq.CompletedAt)
                .ToListAsync();
        }

        public async Task<PlayerQuest?> GetByPlayerAndQuestAsync(Guid playerProfileId, Guid questId)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                .FirstOrDefaultAsync(pq => pq.PlayerProfileId == playerProfileId && pq.QuestId == questId);
        }

        public async Task<PlayerQuest> CreateAsync(PlayerQuest playerQuest)
        {
            await _context.PlayerQuests.AddAsync(playerQuest);
            await _context.SaveChangesAsync();
            return playerQuest;
        }

        public async Task<PlayerQuest> UpdateAsync(PlayerQuest playerQuest)
        {
            _context.PlayerQuests.Update(playerQuest);
            await _context.SaveChangesAsync();
            return playerQuest;
        }

        public async Task<bool> HasQuestAsync(Guid playerProfileId, Guid questId)
        {
            return await _context.PlayerQuests
                .AnyAsync(pq => pq.PlayerProfileId == playerProfileId && pq.QuestId == questId);
        }
    }
}
