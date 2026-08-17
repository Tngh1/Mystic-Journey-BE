namespace DAL.Models
{
    // Initializes a new default instance of the PlayerSkin class.
    public class PlayerSkin
    {
        // Executes player skin id operation.
        public int PlayerSkinId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes skin id operation.
        public int SkinId { get; set; }
        // Executes skin operation.
        public Skin? Skin { get; set; }

        // Executes is equipped operation.
        public bool IsEquipped { get; set; } = false;
        // Executes unlocked at operation.
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}
