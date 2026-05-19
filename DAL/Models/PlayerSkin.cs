namespace DAL.Models
{
    public class PlayerSkin
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public Guid SkinId { get; set; }
        public Skin? Skin { get; set; }

        public bool IsEquipped { get; set; } = false;
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}
