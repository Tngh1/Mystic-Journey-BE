using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using BLL.DTOs;

namespace Mystic_Journey_API.OData
{
    public static class EdmModelBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            // Item - used for manage-items (filter: name, type, rarity)
            var itemSet = builder.EntitySet<ItemResponseDto>("Items");
            itemSet.EntityType.HasKey(x => x.Id);
            itemSet.EntityType.Property(x => x.Name);
            itemSet.EntityType.Property(x => x.Type);
            itemSet.EntityType.Property(x => x.Rarity);
            itemSet.EntityType.Property(x => x.IsActive);

            // Monster - used for manage-monsters (filter: name, type)
            var monsterSet = builder.EntitySet<MonsterResponseDto>("Monsters");
            monsterSet.EntityType.HasKey(x => x.Id);
            monsterSet.EntityType.Property(x => x.Name);
            monsterSet.EntityType.Property(x => x.Type);
            monsterSet.EntityType.Property(x => x.IsActive);

            // ShopItem - used for manage-shop (filter: currency)
            var shopItemSet = builder.EntitySet<ShopItemResponseDto>("ShopItems");
            shopItemSet.EntityType.HasKey(x => x.Id);
            shopItemSet.EntityType.Property(x => x.Currency);
            shopItemSet.EntityType.Property(x => x.IsActive);

            // Quest - used for manage-quests (filter: type)
            var questSet = builder.EntitySet<QuestResponseDto>("Quests");
            questSet.EntityType.HasKey(x => x.Id);
            questSet.EntityType.Property(x => x.Type);
            questSet.EntityType.Property(x => x.IsActive);

            // Achievement - used for manage-achievements (filter: type)
            var achievementSet = builder.EntitySet<AchievementResponseDto>("Achievements");
            achievementSet.EntityType.HasKey(x => x.Id);
            achievementSet.EntityType.Property(x => x.Type);
            achievementSet.EntityType.Property(x => x.IsActive);

            // Content - used for manage-content (filter: isPublished, isActive)
            var contentSet = builder.EntitySet<ContentResponseDto>("Contents");
            contentSet.EntityType.HasKey(x => x.Id);
            contentSet.EntityType.Property(x => x.IsPublished);
            contentSet.EntityType.Property(x => x.IsActive);

            // DungeonConfig - used for manage-dungeons (filter: type)
            var dungeonSet = builder.EntitySet<DungeonConfigResponseDto>("Dungeons");
            dungeonSet.EntityType.HasKey(x => x.Id);
            dungeonSet.EntityType.Property(x => x.Type);
            dungeonSet.EntityType.Property(x => x.IsActive);

            // GameSetting - used for manage-game-config (filter: key)
            var gameSettingSet = builder.EntitySet<GameSettingResponseDto>("GameSettings");
            gameSettingSet.EntityType.HasKey(x => x.Id);
            gameSettingSet.EntityType.Property(x => x.Key);
            gameSettingSet.EntityType.Property(x => x.IsActive);

            // Mail - used for manage-mailbox (filter: type, isRead, isClaimed)
            var mailSet = builder.EntitySet<MailResponseDto>("Mails");
            mailSet.EntityType.HasKey(x => x.Id);
            mailSet.EntityType.Property(x => x.Type);
            mailSet.EntityType.Property(x => x.IsRead);
            mailSet.EntityType.Property(x => x.IsClaimed);

            // GachaBanner - used for manage-gacha-pools (filter: type)
            var gachaBannerSet = builder.EntitySet<GachaBannerResponseDto>("GachaBanners");
            gachaBannerSet.EntityType.HasKey(x => x.Id);
            gachaBannerSet.EntityType.Property(x => x.Type);
            gachaBannerSet.EntityType.Property(x => x.IsActive);

            // Account (Admin view) - used for manage-admins (filter: roleName)
            var accountSet = builder.EntitySet<AccountAdminResponseDto>("Accounts");
            accountSet.EntityType.HasKey(x => x.AccountId);
            accountSet.EntityType.Property(x => x.RoleName);
            accountSet.EntityType.Property(x => x.IsActive);

            // PlayerProfile - used for manage-players (filter: playerClass, displayName)
            var playerProfileSet = builder.EntitySet<PlayerProfileResponseDto>("PlayerProfiles");
            playerProfileSet.EntityType.HasKey(x => x.Id);
            playerProfileSet.EntityType.Property(x => x.PlayerClass);
            playerProfileSet.EntityType.Property(x => x.DisplayName);
            playerProfileSet.EntityType.Property(x => x.IsBanned);

            // PurchaseHistory - used for manage-transactions (filter: playerName)
            var purchaseHistorySet = builder.EntitySet<PurchaseHistoryResponseDto>("PurchaseHistories");
            purchaseHistorySet.EntityType.HasKey(x => x.Id);
            purchaseHistorySet.EntityType.Property(x => x.PlayerName);

            return builder.GetEdmModel();
        }
    }
}
