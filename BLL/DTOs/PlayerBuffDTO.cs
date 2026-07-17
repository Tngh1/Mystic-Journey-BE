namespace BLL.DTOs
{
    public class PlayerBuffDTO
    {
        public string BuffName { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public float DurationRemaining { get; set; }
        public bool IsDebuff { get; set; }
    }
}
