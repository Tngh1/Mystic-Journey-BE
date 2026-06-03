namespace DAL.Models
{
    public class NPCDialogue
    {
        public int NPCDialogueId { get; set; }

        public int NPCId { get; set; }
        public NPC? NPC { get; set; }

        public string Content { get; set; } = string.Empty;

        // ResponseTypes: None, Quest, Shop, Reward, Exit
        public string ResponseType { get; set; } = "None";

        public int? LinkedQuestId { get; set; }
        public Quest? LinkedQuest { get; set; }

        public int? LinkedShopItemId { get; set; }
        public ShopItem? LinkedShopItem { get; set; }

        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
