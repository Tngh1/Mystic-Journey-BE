using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the ISkillService class.
    public interface ISkillService
    {

        Task<PlayerMeSkillsResponseDto> GetMeSkills(int playerProfileId);

        Task<PlayerSkillResponseDto> UpgradePlayerSkill(int actorPlayerProfileId, UpgradePlayerSkillRequestDto request);

        Task<PlayerSkillResponseDto> EquipPlayerSkill(int actorPlayerProfileId, EquipSkillRequestDto request);


        Task<PlayerSkillResponseDto> RecordSkillCast(int actorPlayerProfileId, int playerSkillId);

        Task<PlayerSkillResponseDto?> DismantlePlayerSkill(int actorPlayerProfileId, DismantlePlayerSkillRequestDto request);


        Task<SkillResponseDto?> GetSkillById(int id);

        Task<PagedResultDto<SkillResponseDto>> GetSkillsPaged(int page, int pageSize, string? search, string? type, bool? isActive);

        Task<SkillResponseDto> CreateSkill(CreateSkillRequestDto request);

        Task<SkillResponseDto> UpdateSkill(int id, UpdateSkillRequestDto request);
    }
}
