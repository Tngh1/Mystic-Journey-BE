using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho nhiệm vụ người chơi sử dụng Entity Framework.
    /// </summary>
    public class PlayerQuestRepository : IPlayerQuestRepository
    {
        private readonly MysticJourneyDbContext _context;

        public PlayerQuestRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        /// <summary>Lấy tất cả nhiệm vụ của người chơi, kèm chi tiết nhiệm vụ và phần thưởng, sắp xếp theo thời gian nhận giảm dần.</summary>
        public async Task<List<PlayerQuest>> GetByPlayerId(int playerProfileId)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItem)
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItems)
                        .ThenInclude(r => r.Item)
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardSkills)
                        .ThenInclude(r => r.Skill)
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardSkill)
                .Where(pq => pq.PlayerProfileId == playerProfileId)
                .OrderByDescending(pq => pq.AcceptedAt)
                .ToListAsync();
        }

        /// <summary>Lấy nhiệm vụ của người chơi trên một bản đồ cụ thể, sắp xếp theo cấp độ yêu cầu.</summary>
        public async Task<List<PlayerQuest>> GetByPlayerIdAndMap(int playerProfileId, string mapName)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItem)
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItems)
                        .ThenInclude(r => r.Item)
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardSkills)
                        .ThenInclude(r => r.Skill)
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardSkill)
                .Where(pq =>
                    pq.PlayerProfileId == playerProfileId &&
                    pq.Quest != null &&
                    pq.Quest.MapName == mapName)
                .OrderBy(pq => pq.Quest!.RequiredLevel)
                .ThenBy(pq => pq.QuestId)
                .ToListAsync();
        }

        /// <summary>Lấy một nhiệm vụ cụ thể của người chơi, kèm phần thưởng.</summary>
        public async Task<PlayerQuest?> GetByPlayerAndQuest(int playerProfileId, int questId)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItem)
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItems)
                        .ThenInclude(r => r.Item)
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardSkills)
                        .ThenInclude(r => r.Skill)
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardSkill)
                .FirstOrDefaultAsync(pq =>
                    pq.PlayerProfileId == playerProfileId &&
                    pq.QuestId == questId);
        }

        /// <summary>Lấy nhiều nhiệm vụ của người chơi theo danh sách questId (dùng cho cập nhật tiến độ hàng loạt).</summary>
        public async Task<List<PlayerQuest>> GetByPlayerAndQuestIds(int playerProfileId, List<int> questIds)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItem)
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardItems)
                        .ThenInclude(r => r.Item)
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardSkills)
                        .ThenInclude(r => r.Skill)
                .Include(pq => pq.Quest)
                    .ThenInclude(q => q!.RewardSkill)
                .Where(pq => pq.PlayerProfileId == playerProfileId && questIds.Contains(pq.QuestId))
                .ToListAsync();
        }

        /// <summary>Tạo mới nhiệm vụ cho người chơi (nhận nhiệm vụ).</summary>
        public async Task<PlayerQuest> Create(PlayerQuest entity)
        {
            _context.PlayerQuests.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>Cập nhật nhiệm vụ (tiến độ, trạng thái).</summary>
        public async Task<PlayerQuest> Update(PlayerQuest entity)
        {
            _context.PlayerQuests.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>Cập nhật nhiều nhiệm vụ cùng lúc (batch save).</summary>
        public async Task UpdateRange(List<PlayerQuest> entities)
        {
            _context.PlayerQuests.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}
