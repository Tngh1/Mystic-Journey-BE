using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the UpdateHpRequestDto class.
    public class UpdateHpRequestDto
    {
        // Executes current hp operation.
        [Required]
        [Range(0, int.MaxValue)]
        public int CurrentHp { get; set; }
    }
}
