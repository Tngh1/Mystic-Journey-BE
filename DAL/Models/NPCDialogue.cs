namespace DAL.Models
{
    // Initializes a new default instance of the NPCDialogue class.
    public class NPCDialogue
    {
        // Executes npc dialogue id operation.
        public int NPCDialogueId { get; set; }

        // Executes npc id operation.
        public int NPCId { get; set; }
        // Executes npc operation.
        public NPC? NPC { get; set; }

        // Executes content operation.
        public string Content { get; set; } = string.Empty;

        // Executes response type operation.
        public string ResponseType { get; set; } = "None";

        // Executes linked quest id operation.
        public int? LinkedQuestId { get; set; }
        // Executes linked quest operation.
        public Quest? LinkedQuest { get; set; }

        // Executes linked shop item id operation.
        public int? LinkedShopItemId { get; set; }
        // Executes linked shop item operation.
        public ShopItem? LinkedShopItem { get; set; }

        // Executes display order operation.
        public int DisplayOrder { get; set; } = 0;
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }
}
