using AutoMapper;
using BLL.DTOs;
using DAL.Models;

namespace BLL.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterRequestDto, Account>();

            CreateMap<Account, AccountResponseDto>();

            CreateMap<Item, ItemResponseDto>();
            CreateMap<CreateItemRequestDto, Item>();
            CreateMap<UpdateItemRequestDto, Item>();

            CreateMap<Monster, MonsterResponseDto>();
            CreateMap<CreateMonsterRequestDto, Monster>();
            CreateMap<UpdateMonsterRequestDto, Monster>();

            CreateMap<DungeonConfig, DungeonConfigResponseDto>();
            CreateMap<CreateDungeonConfigRequestDto, DungeonConfig>();
            CreateMap<UpdateDungeonConfigRequestDto, DungeonConfig>();

            CreateMap<ShopItem, ShopItemResponseDto>();
            CreateMap<CreateShopItemRequestDto, ShopItem>();
            CreateMap<UpdateShopItemRequestDto, ShopItem>();

            CreateMap<GachaBanner, GachaBannerResponseDto>();
            CreateMap<CreateGachaBannerRequestDto, GachaBanner>();
            CreateMap<UpdateGachaBannerRequestDto, GachaBanner>();

            CreateMap<GachaBannerItem, GachaBannerItemResponseDto>();
            CreateMap<CreateGachaBannerItemRequestDto, GachaBannerItem>();

            CreateMap<Quest, QuestResponseDto>();
            CreateMap<CreateQuestRequestDto, Quest>();
            CreateMap<UpdateQuestRequestDto, Quest>();

            CreateMap<Achievement, AchievementResponseDto>();
            CreateMap<CreateAchievementRequestDto, Achievement>();
            CreateMap<UpdateAchievementRequestDto, Achievement>();

            CreateMap<GameSetting, GameSettingResponseDto>();
            CreateMap<CreateGameSettingRequestDto, GameSetting>();
            CreateMap<UpdateGameSettingRequestDto, GameSetting>();

            CreateMap<Content, ContentResponseDto>();
            CreateMap<Content, ContentDetailResponseDto>();
            CreateMap<CreateContentRequestDto, Content>();
            CreateMap<UpdateContentRequestDto, Content>();

            CreateMap<BlockContent, BlockContentResponseDto>();
            CreateMap<CreateBlockContentRequestDto, BlockContent>();
            CreateMap<UpdateBlockContentRequestDto, BlockContent>();

            CreateMap<CategoryContent, CategoryContentResponseDto>();
            CreateMap<CreateCategoryContentRequestDto, CategoryContent>();

            CreateMap<Mail, MailResponseDto>();
            CreateMap<SendMailRequestDto, Mail>();
            CreateMap<BulkSendMailRequestDto, Mail>();

            CreateMap<PlayerProfile, PlayerProfileResponseDto>();
            CreateMap<PlayerProfile, PlayerProfileDetailResponseDto>();
            CreateMap<UpdatePlayerProfileRequestDto, PlayerProfile>();

            CreateMap<PlayerStat, PlayerStatsResponseDto>();

            CreateMap<PurchaseHistory, PurchaseHistoryResponseDto>();
            CreateMap<CreateAccountAdminRequestDto, Account>();
            CreateMap<UpdateAccountAdminRequestDto, Account>();

            CreateMap<Account, AccountAdminResponseDto>();

            CreateMap<DailyLoginReward, DailyLoginRewardResponseDto>();
            CreateMap<CreateDailyLoginRewardRequestDto, DailyLoginReward>();

            CreateMap<MonsterDrop, MonsterDropResponseDto>();
            CreateMap<CreateMonsterDropRequestDto, MonsterDrop>();

            // Guild
            CreateMap<Guild, GuildResponseDto>();
            CreateMap<Guild, GuildDetailResponseDto>();
            CreateMap<CreateGuildRequestDto, Guild>();
            CreateMap<UpdateGuildRequestDto, Guild>();
            CreateMap<GuildMember, GuildMemberResponseDto>();
            CreateMap<UpdateGuildMemberRequestDto, GuildMember>();
            CreateMap<GuildInvitation, GuildInvitationResponseDto>();
            CreateMap<CreateGuildInvitationRequestDto, GuildInvitation>();
            CreateMap<RespondGuildInvitationRequestDto, GuildInvitation>();

            // Chat & Friend
            CreateMap<ChatMessage, ChatMessageResponseDto>();
            CreateMap<SendChatMessageRequestDto, ChatMessage>();
            CreateMap<Friend, FriendResponseDto>();

            // Chest
            CreateMap<Chest, ChestResponseDto>();
            CreateMap<CreateChestRequestDto, Chest>();
            CreateMap<UpdateChestRequestDto, Chest>();
            CreateMap<ChestItem, ChestItemResponseDto>();
            CreateMap<CreateChestItemRequestDto, ChestItem>();
            CreateMap<PlayerChest, PlayerChestResponseDto>();

            // Skill
            CreateMap<Skill, SkillResponseDto>();
            CreateMap<CreateSkillRequestDto, Skill>();
            CreateMap<UpdateSkillRequestDto, Skill>();
            CreateMap<PlayerSkill, PlayerSkillResponseDto>();

            // Skin
            CreateMap<Skin, SkinResponseDto>();
            CreateMap<CreateSkinRequestDto, Skin>();
            CreateMap<UpdateSkinRequestDto, Skin>();
            CreateMap<PlayerSkin, PlayerSkinResponseDto>();

            // PlayerQuest
            CreateMap<PlayerQuest, PlayerQuestResponseDto>();

            // PlayerDailyLogin
            CreateMap<PlayerDailyLogin, PlayerDailyLoginResponseDto>();

            // PlayerCurrencyLog
            CreateMap<PlayerCurrencyLog, PlayerCurrencyLogResponseDto>();

            // PlayerAchievement
            CreateMap<PlayerAchievement, PlayerAchievementResponseDto>();

            // NPC
            CreateMap<NPC, NPCResponseDto>();
            CreateMap<CreateNPCRequestDto, NPC>();
            CreateMap<UpdateNPCRequestDto, NPC>();
            CreateMap<NPCDialogue, NPCDialogueResponseDto>();
            CreateMap<CreateNPCDialogueRequestDto, NPCDialogue>();
            CreateMap<UpdateNPCDialogueRequestDto, NPCDialogue>();

            // GachaPullHistory
            CreateMap<GachaPullHistory, GachaPullHistoryResponseDto>();

            // Announcement
            CreateMap<GameAnnouncement, GameAnnouncementResponseDto>();
            CreateMap<CreateGameAnnouncementRequestDto, GameAnnouncement>();
            CreateMap<UpdateGameAnnouncementRequestDto, GameAnnouncement>();
            CreateMap<PlayerAnnouncement, PlayerAnnouncementResponseDto>();

            // Inventory
            CreateMap<InventoryItem, InventoryItemResponseDto>();
            CreateMap<AddInventoryItemRequestDto, InventoryItem>();
            CreateMap<UpdateInventoryItemRequestDto, InventoryItem>();

            // EquipmentStats
            CreateMap<EquipmentStats, EquipmentStatsResponseDto>();
            CreateMap<CreateEquipmentStatsRequestDto, EquipmentStats>();
            CreateMap<UpdateEquipmentStatsRequestDto, EquipmentStats>();
        }
    }
}
