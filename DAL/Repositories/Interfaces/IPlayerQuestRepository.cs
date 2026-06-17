using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IPlayerQuestRepository
    {
        /// <summary>Lấy tất cả PlayerQuest của một player.</summary>
        Task<List<PlayerQuest>> GetByPlayerId(int playerProfileId);

        Task<List<PlayerQuest>> GetByPlayerIdAndMap(int playerProfileId, string mapName);

        /// <summary>Lấy một PlayerQuest cụ thể của player.</summary>
        Task<PlayerQuest?> GetByPlayerAndQuest(int playerProfileId, int questId);

        /// <summary>Lấy nhiều PlayerQuest theo danh sách questId (dùng cho batch-progress).</summary>
        Task<List<PlayerQuest>> GetByPlayerAndQuestIds(int playerProfileId, List<int> questIds);

        /// <summary>Tạo mới PlayerQuest (Accept quest).</summary>
        Task<PlayerQuest> Create(PlayerQuest entity);

        /// <summary>Cập nhật PlayerQuest (progress, status).</summary>
        Task<PlayerQuest> Update(PlayerQuest entity);

        /// <summary>Cập nhật nhiều PlayerQuest cùng lúc (batch save).</summary>
        Task UpdateRange(List<PlayerQuest> entities);
    }
}
