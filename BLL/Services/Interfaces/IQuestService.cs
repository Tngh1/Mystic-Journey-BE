using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IQuestService class.
    public interface IQuestService
    {

        Task<QuestResponseDto?> GetQuestById(int id);

        Task<PagedResultDto<QuestResponseDto>> GetQuestsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? mapName, string? sortBy = null, string? sortOrder = null);
        Task<List<NPCResponseDto>> GetQuestNpcOptions(string? mapName);



        Task<QuestResponseDto> CreateQuest(UpdateQuestRequestDto request);

        Task<QuestResponseDto> UpdateQuest(int id, UpdateQuestRequestDto request);
    }
}
