using System.Collections.Generic;

namespace BLL.DTOs
{
    // Initializes a new default instance of the UpdatePlayerBuffsRequest class.
    public class UpdatePlayerBuffsRequest
    {
        // Executes buffs operation.
        public List<PlayerBuffDTO> Buffs { get; set; } = new List<PlayerBuffDTO>();
    }
}
