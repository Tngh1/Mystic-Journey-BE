using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho thành tích người chơi sử dụng Entity Framework.
    /// </summary>
    public class PlayerAchievementRepository : IPlayerAchievementRepository
    {
        private readonly MysticJourneyDbContext _context;

        public PlayerAchievementRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        /// <summary>Lấy danh sách thành tích đã đạt được của người chơi, kèm thông tin thành tích.</summary>
        public async Task<List<PlayerAchievement>> GetByPlayerProfileId(int playerProfileId)
        {
            return await _context.PlayerAchievements
                .Include(pa => pa.Achievement)
                    .ThenInclude(a => a!.RewardItem)
                .Where(pa => pa.PlayerProfileId == playerProfileId)
                .ToListAsync();
        }

        /// <summary>Lấy một thành tích người chơi theo ID, kèm thông tin thành tích.</summary>
        public async Task<PlayerAchievement?> GetByIdWithAchievement(int playerAchievementId)
        {
            return await _context.PlayerAchievements
                .Include(pa => pa.Achievement)
                    .ThenInclude(a => a!.RewardItem)
                .FirstOrDefaultAsync(pa => pa.PlayerAchievementId == playerAchievementId);
        }

        /// <summary>Cập nhật trạng thái thành tích người chơi.</summary>
        public async Task<PlayerAchievement> Update(PlayerAchievement playerAchievement)
        {
            _context.PlayerAchievements.Update(playerAchievement);
            await _context.SaveChangesAsync();
            return playerAchievement;
        }

        /// <summary>Cập nhật nhiều thành tích trong một lần SaveChanges (tính lại Progress cho cả bảng).</summary>
        public async Task UpdateRange(IEnumerable<PlayerAchievement> achievements)
        {
            _context.PlayerAchievements.UpdateRange(achievements);
            await _context.SaveChangesAsync();
        }

        /// <summary>Thêm nhiều thành tích người chơi cùng lúc.</summary>
        public async Task AddRange(IEnumerable<PlayerAchievement> achievements)
        {
            await _context.PlayerAchievements.AddRangeAsync(achievements);
            await _context.SaveChangesAsync();
        }
    }
}
