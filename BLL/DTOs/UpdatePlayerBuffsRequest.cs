using System.Collections.Generic;

namespace BLL.DTOs
{
    public class UpdatePlayerBuffsRequest
    {
        public List<PlayerBuffDTO> Buffs { get; set; } = new List<PlayerBuffDTO>();
    }
}
