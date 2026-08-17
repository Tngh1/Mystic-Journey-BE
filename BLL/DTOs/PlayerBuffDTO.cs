using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the PlayerBuffDTO class.
    public class PlayerBuffDTO
    {
        // Executes buff name operation.
        [Required(ErrorMessage = "BuffName is required.")]
        [MaxLength(100, ErrorMessage = "BuffName must not exceed 100 characters.")]
        public string BuffName { get; set; } = string.Empty;

        // Executes icon name operation.
        [MaxLength(100, ErrorMessage = "IconName must not exceed 100 characters.")]
        public string IconName { get; set; } = string.Empty;

        // Executes duration remaining operation.
        [Range(0, float.MaxValue, ErrorMessage = "DurationRemaining cannot be negative.")]
        public float DurationRemaining { get; set; }

        // Executes is debuff operation.
        public bool IsDebuff { get; set; }
    }
}
