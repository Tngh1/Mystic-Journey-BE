using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ISkillService
    {
        Task<PagedResultDto<SkillResponseDto>> GetSkillsPaged(int page, int pageSize, string? search, string? type, bool? isActive);
        Task<SkillResponseDto?> GetSkillById(int id);
        Task<SkillResponseDto> CreateSkill(CreateSkillRequestDto request);
        Task<SkillResponseDto> UpdateSkill(int id, UpdateSkillRequestDto request);

        Task<PlayerSkillResponseDto> UpgradePlayerSkill(int actorPlayerProfileId, UpgradePlayerSkillRequestDto request);
        Task<PlayerSkillResponseDto> EquipPlayerSkill(int actorPlayerProfileId, EquipSkillRequestDto request);
        Task<PlayerSkillResponseDto> UnlockPlayerSkill(int actorPlayerProfileId, UnlockPlayerSkillRequestDto request);
        Task<PlayerSkillResponseDto?> DismantlePlayerSkill(int actorPlayerProfileId, DismantlePlayerSkillRequestDto request);
    }
}
