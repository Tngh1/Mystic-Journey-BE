using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<PlayerQuest>> GetByPlayerId(int playerProfileId)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItem)
                .Where(pq => pq.PlayerProfileId == playerProfileId)
                .OrderByDescending(pq => pq.AcceptedAt)
                .ToListAsync();
        }

        public async Task<List<PlayerQuest>> GetByPlayerIdAndMap(int playerProfileId, string mapName)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItem)
                .Where(pq =>
                    pq.PlayerProfileId == playerProfileId &&
                    pq.Quest != null &&
                    pq.Quest.MapName == mapName)
                .OrderBy(pq => pq.Quest!.RequiredLevel)
                .ThenBy(pq => pq.QuestId)
                .ToListAsync();
        }

        public async Task<PlayerQuest?> GetByPlayerAndQuest(int playerProfileId, int questId)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItem)
                .FirstOrDefaultAsync(pq =>
                    pq.PlayerProfileId == playerProfileId &&
                    pq.QuestId == questId);
        }

        public async Task<List<PlayerQuest>> GetByPlayerAndQuestIds(int playerProfileId, List<int> questIds)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItem)
                .Where(pq => pq.PlayerProfileId == playerProfileId && questIds.Contains(pq.QuestId))
                .ToListAsync();
        }

        public async Task<PlayerQuest> Create(PlayerQuest entity)
        {
            _context.PlayerQuests.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<PlayerQuest> Update(PlayerQuest entity)
        {
            _context.PlayerQuests.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateRange(List<PlayerQuest> entities)
        {
            _context.PlayerQuests.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}
