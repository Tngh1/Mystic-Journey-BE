using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class PlayerBuffDTO
    {
        // MaxLength(100) khop voi cot PlayerBuff.BuffName/IconName trong DAL:
        // SyncBuffs ghi thang gia tri nay vao DB, chuoi dai hon se nem o SaveChangesAsync thanh 500.
        [Required(ErrorMessage = "BuffName is required.")]
        [MaxLength(100, ErrorMessage = "BuffName must not exceed 100 characters.")]
        public string BuffName { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "IconName must not exceed 100 characters.")]
        public string IconName { get; set; } = string.Empty;

        [Range(0, float.MaxValue, ErrorMessage = "DurationRemaining cannot be negative.")]
        public float DurationRemaining { get; set; }

        public bool IsDebuff { get; set; }
    }
}
