using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho chỉ số người chơi và snapshot sử dụng Entity Framework.
    /// </summary>
    public class PlayerStatRepository : IPlayerStatRepository
    {
        private readonly MysticJourneyDbContext _context;

        public PlayerStatRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // ── PlayerStat ──

        /// <summary>Tìm chỉ số người chơi theo mã hồ sơ, trả về null nếu chưa được tạo.</summary>
        public async Task<PlayerStat?> GetByPlayerProfileId(int playerProfileId)
        {
            return await _context.PlayerStats
                .FirstOrDefaultAsync(s => s.PlayerProfileId == playerProfileId);
        }

        /// <summary>Tạo bản ghi chỉ số người chơi mới (tự động ghi nhận thời gian tạo).</summary>
        public async Task<PlayerStat> Create(PlayerStat stat)
        {
            stat.CreatedAt = DateTime.UtcNow;
            await _context.PlayerStats.AddAsync(stat);
            await _context.SaveChangesAsync();
            return stat;
        }

        /// <summary>Cập nhật chỉ số người chơi (tự động ghi nhận thời gian cập nhật).</summary>
        public async Task<PlayerStat> Update(PlayerStat stat)
        {
            stat.UpdatedAt = DateTime.UtcNow;
            _context.PlayerStats.Update(stat);
            await _context.SaveChangesAsync();
            return stat;
        }

        // ── Snapshot ──

        /// <summary>Lấy snapshot chỉ số của người chơi theo mã hồ sơ.</summary>
        public async Task<PlayerStatsSnapshot?> GetSnapshotByPlayerProfileId(int playerProfileId)
        {
            return await _context.PlayerStatsSnapshots
                .FirstOrDefaultAsync(s => s.PlayerProfileId == playerProfileId);
        }

        /// <summary>Tạo snapshot chỉ số người chơi.</summary>
        public async Task<PlayerStatsSnapshot> CreateSnapshot(PlayerStatsSnapshot snapshot)
        {
            await _context.PlayerStatsSnapshots.AddAsync(snapshot);
            await _context.SaveChangesAsync();
            return snapshot;
        }

        /// <summary>Cập nhật snapshot chỉ số người chơi.</summary>
        public async Task<PlayerStatsSnapshot> UpdateSnapshot(PlayerStatsSnapshot snapshot)
        {
            _context.PlayerStatsSnapshots.Update(snapshot);
            await _context.SaveChangesAsync();
            return snapshot;
        }
    }
}
