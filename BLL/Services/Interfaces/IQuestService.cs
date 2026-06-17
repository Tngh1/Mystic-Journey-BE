using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IQuestService
    {
        Task<QuestResponseDto?> GetQuestById(int id);
        Task<QuestResponseDto> CreateQuest(CreateQuestRequestDto request);
        Task<QuestResponseDto> UpdateQuest(int id, UpdateQuestRequestDto request);
        Task<PagedResultDto<QuestResponseDto>> GetQuestsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? mapName);
    }
}
