namespace DAL.Models
{
    public class PlayerSkin
    {
        public int Id { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int SkinId { get; set; }
        public Skin? Skin { get; set; }

        public bool IsEquipped { get; set; } = false;
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}
