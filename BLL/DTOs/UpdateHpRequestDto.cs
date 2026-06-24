using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class UpdateHpRequestDto
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int CurrentHp { get; set; }
    }
}
