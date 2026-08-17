using AutoMapper;
using BLL.DTOs;
using DAL.Models;

namespace BLL.Mappings
{
    // Executes profile operation.
    // Validates input parameters against null or empty values.
    public class AutoMapperProfile : Profile
    {
        // Initializes a new default instance of the AutoMapperProfile class.
        public AutoMapperProfile()
        {

            CreateMap<RegisterRequestDto, Account>();
            CreateMap<AuthResponseDto, AuthResponseDto>();

            CreateMap<Account, AuthResponseDto>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Player"))
                .ForMember(dest => dest.HasCharacter, opt => opt.MapFrom(src => src.PlayerProfile != null && !string.IsNullOrWhiteSpace(src.PlayerProfile.Class)))
                .ForMember(dest => dest.PlayerProfileId, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.PlayerProfileId : (int?)null))
                .ForMember(dest => dest.PlayerDisplayName, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.DisplayName : (string?)null))
                .ForMember(dest => dest.PlayerClass, opt => opt.MapFrom(src => src.PlayerProfile != null && !string.IsNullOrWhiteSpace(src.PlayerProfile.Class) ? src.PlayerProfile.Class.Trim() : null))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.Level : 1))
                .ForMember(dest => dest.LastMapName, opt => opt.MapFrom(src => src.PlayerProfile != null && !string.IsNullOrWhiteSpace(src.PlayerProfile.LastMapName) ? src.PlayerProfile.LastMapName : string.Empty))
                .ForMember(dest => dest.PositionX, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.PositionX : 0.0))
                .ForMember(dest => dest.PositionY, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.PositionY : 0.0))
                .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
                .ForMember(dest => dest.AccessTokenExpiresAt, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiresAt, opt => opt.Ignore());

            CreateMap<Account, MeResponseDto>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Player"))
                .ForMember(dest => dest.PlayerProfileId, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.PlayerProfileId : (int?)null))
                .ForMember(dest => dest.HasCharacter, opt => opt.MapFrom(src => src.PlayerProfile != null && !string.IsNullOrWhiteSpace(src.PlayerProfile.Class)))
                .ForMember(dest => dest.PlayerClass, opt => opt.MapFrom(src => src.PlayerProfile != null && !string.IsNullOrWhiteSpace(src.PlayerProfile.Class) ? src.PlayerProfile.Class.Trim() : null))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.Level : 1))
                .ForMember(dest => dest.LastMapName, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.LastMapName : string.Empty))
                .ForMember(dest => dest.PositionX, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.PositionX : 0.0))
                .ForMember(dest => dest.PositionY, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.PositionY : 0.0));


            CreateMap<Item, ItemResponseDto>();
            CreateMap<UpdateItemRequestDto, Item>();


            CreateMap<Monster, MonsterResponseDto>();
            CreateMap<Monster, MonsterDetailResponseDto>()
                .IncludeBase<Monster, MonsterResponseDto>();
            CreateMap<UpdateMonsterRequestDto, Monster>();


            CreateMap<DungeonConfig, DungeonConfigResponseDto>();
            CreateMap<UpdateDungeonConfigRequestDto, DungeonConfig>();


            CreateMap<ShopItem, ShopItemResponseDto>();
            CreateMap<ShopItem, ShopItemPublicResponseDto>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : string.Empty))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Item != null ? src.Item.Description : null))
                .ForMember(dest => dest.ItemIconUrl, opt => opt.MapFrom(src => src.Item != null ? src.Item.IconUrl : null))
                .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => src.Item != null ? src.Item.Type : string.Empty))
                .ForMember(dest => dest.Rarity, opt => opt.MapFrom(src => src.Item != null ? src.Item.Rarity : string.Empty))
                .ForMember(dest => dest.Slot, opt => opt.MapFrom(src => src.Item != null ? src.Item.Slot : string.Empty))
                .ForMember(dest => dest.MaxStack, opt => opt.MapFrom(src => src.Item != null ? src.Item.MaxStack : 0))
                .ForMember(dest => dest.IsUnlimitedStock, opt => opt.MapFrom(src => src.Stock < 0))
                .ForMember(dest => dest.OriginalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.PurchasedToday, opt => opt.Ignore())
                .ForMember(dest => dest.RemainingDailyPurchases, opt => opt.Ignore())
                .ForMember(dest => dest.CanPurchase, opt => opt.Ignore())
                .ForMember(dest => dest.UnavailableReason, opt => opt.Ignore())
                .ForMember(dest => dest.BaseHp, opt => opt.MapFrom(src => src.Item != null && src.Item.EquipmentStats != null ? src.Item.EquipmentStats.BaseHp : 0))
                .ForMember(dest => dest.BaseAtk, opt => opt.MapFrom(src => src.Item != null && src.Item.EquipmentStats != null ? src.Item.EquipmentStats.BaseAtk : 0))
                .ForMember(dest => dest.BaseDef, opt => opt.MapFrom(src => src.Item != null && src.Item.EquipmentStats != null ? src.Item.EquipmentStats.BaseDef : 0))
                .ForMember(dest => dest.BonusHp, opt => opt.MapFrom(src => src.Item != null && src.Item.EquipmentStats != null ? src.Item.EquipmentStats.BonusHp : 0))
                .ForMember(dest => dest.BonusAtk, opt => opt.MapFrom(src => src.Item != null && src.Item.EquipmentStats != null ? src.Item.EquipmentStats.BonusAtk : 0))
                .ForMember(dest => dest.BonusDef, opt => opt.MapFrom(src => src.Item != null && src.Item.EquipmentStats != null ? src.Item.EquipmentStats.BonusDef : 0))
                .ForMember(dest => dest.BonusCritRate, opt => opt.MapFrom(src =>
                    src.Item != null && src.Item.EquipmentStats != null
                        ? BLL.Utils.StatHelper.FromScaled(src.Item.EquipmentStats.BonusCritRate, BLL.Utils.StatScale.CritRate)
                        : 0f))
                .ForMember(dest => dest.BonusCritDamage, opt => opt.MapFrom(src =>
                    src.Item != null && src.Item.EquipmentStats != null
                        ? BLL.Utils.StatHelper.FromScaled(src.Item.EquipmentStats.BonusCritDamage, BLL.Utils.StatScale.CritRate)
                        : 0f));
            CreateMap<CreateShopItemRequestDto, ShopItem>();
            CreateMap<UpdateShopItemRequestDto, ShopItem>();


            CreateMap<GachaBanner, GachaBannerResponseDto>();
            CreateMap<GachaBanner, GachaBannerDetailResponseDto>()
                .IncludeBase<GachaBanner, GachaBannerResponseDto>()
                .ForMember(dest => dest.BannerItems, opt => opt.MapFrom(src => src.BannerItems));
            CreateMap<UpdateGachaBannerRequestDto, GachaBanner>();

            CreateMap<GachaBannerItem, GachaBannerItemResponseDto>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : null))
                .ForMember(dest => dest.ItemIconUrl, opt => opt.MapFrom(src => src.Item != null ? src.Item.IconUrl : null))
                .ForMember(dest => dest.ItemRarity, opt => opt.MapFrom(src => src.Item != null ? src.Item.Rarity : null));
            CreateMap<CreateGachaBannerItemRequestDto, GachaBannerItem>();


            CreateMap<QuestRewardItem, QuestRewardItemDto>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : null))
                .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.Item != null ? src.Item.IconUrl : null));
            CreateMap<QuestRewardSkill, QuestRewardSkillDto>()
                .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.Name : null))
                .ForMember(dest => dest.ClassRequirement, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.ClassRequirement : null))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.Type : null))
                .ForMember(dest => dest.DamageType, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.DamageType : null));
            CreateMap<Quest, QuestResponseDto>()
                .ForMember(dest => dest.RewardItems, opt => opt.MapFrom(src => src.RewardItems))
                .ForMember(dest => dest.RewardSkills, opt => opt.MapFrom(src => src.RewardSkills));
            CreateMap<UpdateQuestRequestDto, Quest>();


            CreateMap<Achievement, AchievementResponseDto>();
            CreateMap<UpdateAchievementRequestDto, Achievement>();


            CreateMap<Content, ContentResponseDto>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryContentId))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryContent != null ? src.CategoryContent.Name : null));
            CreateMap<Content, ContentDetailResponseDto>()
                .IncludeBase<Content, ContentResponseDto>()
                .ForMember(dest => dest.Blocks, opt => opt.MapFrom(src => src.BlockContents ?? new List<BlockContent>()));
            CreateMap<UpdateContentRequestDto, Content>();

            CreateMap<BlockContent, BlockContentResponseDto>();
            CreateMap<CreateBlockContentRequestDto, BlockContent>();
            CreateMap<UpdateBlockContentRequestDto, BlockContent>();

            CreateMap<CategoryContent, CategoryContentResponseDto>();
            CreateMap<CreateCategoryContentRequestDto, CategoryContent>();


            CreateMap<PlayerProfile, PlayerProfileResponseDto>()
                .ForMember(dest => dest.AccountEmail, opt => opt.MapFrom(src => src.Account != null ? src.Account.Email : null))
                .ForMember(dest => dest.PlayerClass, opt => opt.MapFrom(src => src.Class))
                .ForMember(dest => dest.Energy, opt => opt.MapFrom(src => src.CurrentEnergy))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.AvatarUrl) ? null : src.AvatarUrl))
                .ForMember(dest => dest.IsBanned, opt => opt.MapFrom(src => src.Account != null && !src.Account.IsActive));
            CreateMap<PlayerProfile, PlayerProfileDetailResponseDto>()
                .IncludeBase<PlayerProfile, PlayerProfileResponseDto>()
                .ForMember(dest => dest.Stats, opt => opt.MapFrom(src => src.PlayerStats));
            CreateMap<UpdatePlayerProfileRequestDto, PlayerProfile>()
                .ForMember(dest => dest.CurrentEnergy, opt => opt.MapFrom(src => src.Energy));

            CreateMap<PlayerStat, PlayerStatsResponseDto>();

            CreateMap<ClassConfig, ClassConfigResponseDto>();


            CreateMap<PurchaseHistory, PurchaseHistoryResponseDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.DisplayName : null))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.ShopItem != null && src.ShopItem.Item != null ? src.ShopItem.Item.Name : null))
                .ForMember(dest => dest.ItemIconUrl, opt => opt.MapFrom(src => src.ShopItem != null && src.ShopItem.Item != null ? src.ShopItem.Item.IconUrl : null))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.ShopItem != null ? src.ShopItem.Currency : "Unknown"));


            CreateMap<Account, AccountAdminResponseDto>()
                .ForMember(dest => dest.PlayerProfileId, opt => opt.MapFrom(src => src.PlayerProfile != null ? (int?)src.PlayerProfile.PlayerProfileId : null))
                .ForMember(dest => dest.PlayerDisplayName, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.DisplayName : null))
                .ForMember(dest => dest.PlayerClass, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.Class : null))
                .ForMember(dest => dest.PlayerLevel, opt => opt.MapFrom(src => src.PlayerProfile != null ? (int?)src.PlayerProfile.Level : null));


            CreateMap<DailyLoginReward, DailyLoginRewardResponseDto>()
                .ForMember(dest => dest.RewardItemName, opt => opt.MapFrom(src => src.RewardItem != null ? src.RewardItem.Name : null));
            CreateMap<CreateDailyLoginRewardRequestDto, DailyLoginReward>();


            CreateMap<MonsterDrop, MonsterDropResponseDto>();
            CreateMap<CreateMonsterDropRequestDto, MonsterDrop>();


            CreateMap<MonsterSpawn, MonsterSpawnResponseDto>()
                .ForMember(dest => dest.MonsterName, opt => opt.MapFrom(src => src.Monster != null ? src.Monster.Name : string.Empty))
                .ForMember(dest => dest.MonsterType, opt => opt.MapFrom(src => src.Monster != null ? src.Monster.Type : string.Empty))
                .ForMember(dest => dest.DungeonName, opt => opt.MapFrom(src => src.Dungeon != null ? src.Dungeon.Name : null))
                .ForMember(dest => dest.IsDungeonRepeatable, opt => opt.MapFrom(src => src.Dungeon != null ? src.Dungeon.IsRepeatable : true));


            CreateMap<MailboxRewardItem, MailboxRewardItemDto>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : null))
                .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.Item != null ? src.Item.IconUrl : null));

            CreateMap<Mailbox, MailboxDetailDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.DisplayName : null))
                .ForMember(dest => dest.AttachedItems, opt => opt.MapFrom(src => src.AttachedItems ?? new List<MailboxRewardItem>()));

            CreateMap<Mailbox, MailboxSummaryDto>()
                .ForMember(dest => dest.HasClaimableReward, opt => opt.MapFrom(src => !src.IsClaimed
                    && (src.AttachedGold > 0
                        || src.AttachedGems > 0
                        || (src.AttachedItems != null && src.AttachedItems.Any(i => i.Quantity > 0)))))
                .ForMember(dest => dest.RemainingDays, opt => opt.MapFrom(src => src.ExpiredAt.HasValue ? (int?)Math.Max(0, (int)(src.ExpiredAt.Value - DateTime.UtcNow).TotalDays) : null));


            CreateMap<Guild, GuildResponseDto>();
            CreateMap<Guild, GuildDetailResponseDto>();
            CreateMap<CreateGuildRequestDto, Guild>();
            CreateMap<UpdateGuildRequestDto, Guild>();

            CreateMap<GuildMember, GuildMemberResponseDto>();

            CreateMap<GuildInvitation, GuildInvitationResponseDto>();


            CreateMap<ChatMessage, ChatMessageResponseDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.DisplayName : null))
                .ForMember(dest => dest.SenderAvatarUrl, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.AvatarUrl : null))
                .ForMember(dest => dest.RecipientName, opt => opt.MapFrom(src => src.Recipient != null ? src.Recipient.DisplayName : null))
                .ForMember(dest => dest.RecipientAvatarUrl, opt => opt.MapFrom(src => src.Recipient != null ? src.Recipient.AvatarUrl : null));
            CreateMap<SendChatMessageRequestDto, ChatMessage>()
                .ForMember(dest => dest.ChatMessageId, opt => opt.Ignore())
                .ForMember(dest => dest.SenderId, opt => opt.Ignore())
                .ForMember(dest => dest.Sender, opt => opt.Ignore())
                .ForMember(dest => dest.Recipient, opt => opt.Ignore())
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content.Trim()))
                .ForMember(dest => dest.SentAt, opt => opt.Ignore());
            CreateMap<WorldChatMessage, WorldChatMessageResponseDto>()
                .ForMember(dest => dest.ChatMessageId, opt => opt.MapFrom(src => src.WorldChatMessageId))
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.DisplayName : null))
                .ForMember(dest => dest.SenderAvatarUrl, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.AvatarUrl : null))
                .ForMember(dest => dest.Channel, opt => opt.MapFrom(src => "World"));
            CreateMap<SendWorldChatMessageRequestDto, WorldChatMessage>()
                .ForMember(dest => dest.WorldChatMessageId, opt => opt.Ignore())
                .ForMember(dest => dest.SenderId, opt => opt.Ignore())
                .ForMember(dest => dest.Sender, opt => opt.Ignore())
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content.Trim()))
                .ForMember(dest => dest.SentAt, opt => opt.Ignore());
            CreateMap<Friend, FriendResponseDto>();


            CreateMap<Chest, ChestResponseDto>();
            CreateMap<CreateChestRequestDto, Chest>();
            CreateMap<UpdateChestRequestDto, Chest>();

            CreateMap<ChestItem, ChestItemResponseDto>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : null))
                .ForMember(dest => dest.ItemIconUrl, opt => opt.MapFrom(src => src.Item != null ? src.Item.IconUrl : null))
                .ForMember(dest => dest.ItemRarity, opt => opt.MapFrom(src => src.Item != null ? src.Item.Rarity : null));
            CreateMap<CreateChestItemRequestDto, ChestItem>();

            CreateMap<PlayerChest, PlayerChestResponseDto>();


            CreateMap<Skill, SkillResponseDto>();
            CreateMap<CreateSkillRequestDto, Skill>();
            CreateMap<UpdateSkillRequestDto, Skill>();

            CreateMap<PlayerSkill, PlayerSkillResponseDto>()
                .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.Name : string.Empty))
                .ForMember(dest => dest.SkillDescription, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.Description : null))
                .ForMember(dest => dest.SkillType, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.Type : string.Empty))
                .ForMember(dest => dest.DamageType, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.DamageType : string.Empty))
                .ForMember(dest => dest.TargetType, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.TargetType : string.Empty))
                .ForMember(dest => dest.CooldownSeconds, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.CooldownSeconds : 0))
                .ForMember(dest => dest.BaseDamage, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.BaseDamage : 0.0))
                .ForMember(dest => dest.EffectiveDamage, opt => opt.MapFrom(src =>
                    src.Skill != null ?
                    src.Skill.BaseDamage * (1 + src.Skill.DamageGrowthPercent / 100.0 * (src.Level - 1)) + src.Skill.DamagePerLevel * (src.Level - 1)
                    : 0.0))
                .ForMember(dest => dest.UnlockLevel, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.UnlockLevel : 1))
                .ForMember(dest => dest.CorruptionCost, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.CorruptionCost : 0f));


            CreateMap<Skin, SkinResponseDto>();
            CreateMap<CreateSkinRequestDto, Skin>();
            CreateMap<UpdateSkinRequestDto, Skin>();
            CreateMap<PlayerSkin, PlayerSkinResponseDto>();


            CreateMap<PlayerQuest, PlayerQuestResponseDto>()
                .ForMember(dest => dest.QuestTitle, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.Title : string.Empty))
                .ForMember(dest => dest.QuestDescription, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.Description : null))
                .ForMember(dest => dest.QuestType, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.Type : "Main"))
                .ForMember(dest => dest.MapName, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.MapName : "ElfForest"))
                .ForMember(dest => dest.RegionName, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.RegionName : null))
                .ForMember(dest => dest.ObjectiveType, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.ObjectiveType : "Explore"))
                .ForMember(dest => dest.ObjectiveTarget, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.ObjectiveTarget : null))
                .ForMember(dest => dest.ObjectiveLocation, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.ObjectiveLocation : null))
                .ForMember(dest => dest.QuestGiverName, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.QuestGiverName : null))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => Math.Max(1, src.TargetValue)))
                .ForMember(dest => dest.TargetAmount, opt => opt.MapFrom(src => Math.Max(1, src.Quest != null ? src.Quest.TargetAmount : src.TargetValue)))
                .ForMember(dest => dest.RequiredLevel, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.RequiredLevel : 1))
                .ForMember(dest => dest.RewardExperience, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.RewardExperience : 0))
                .ForMember(dest => dest.RewardGold, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.RewardGold : 0))
                .ForMember(dest => dest.RewardGems, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.RewardGems : 0))
                .ForMember(dest => dest.RewardItemId, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.RewardItemId : null))
                .ForMember(dest => dest.RewardItemName, opt => opt.MapFrom(src => src.Quest != null && src.Quest.RewardItem != null ? src.Quest.RewardItem.Name : null))
                .ForMember(dest => dest.RewardItems, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.RewardItems : Enumerable.Empty<QuestRewardItem>()))
                .ForMember(dest => dest.RewardSkills, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.RewardSkills : Enumerable.Empty<QuestRewardSkill>()))
                .ForMember(dest => dest.RewardSkillId, opt => opt.MapFrom(src => src.Quest != null ? src.Quest.RewardSkillId : null))
                .ForMember(dest => dest.RewardSkillName, opt => opt.MapFrom(src => src.Quest != null && src.Quest.RewardSkill != null ? src.Quest.RewardSkill.Name : null));


            CreateMap<PlayerDailyLogin, PlayerDailyLoginResponseDto>();


            CreateMap<PlayerCurrencyLog, PlayerCurrencyLogResponseDto>();
            CreateMap<PlayerProfile, CurrencyBalanceResponseDto>()
                .ForMember(dest => dest.ServerTimeUtc, opt => opt.Ignore());

            CreateMap<PlayerAchievement, PlayerAchievementResponseDto>()
                .ForMember(dest => dest.AchievementName, opt => opt.MapFrom(src => src.Achievement != null ? src.Achievement.Name : ""))
                .ForMember(dest => dest.AchievementDescription, opt => opt.MapFrom(src => src.Achievement != null ? src.Achievement.Description : null))
                .ForMember(dest => dest.AchievementType, opt => opt.MapFrom(src => src.Achievement != null ? src.Achievement.Type : ""))
                .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.Achievement != null ? src.Achievement.IconUrl : null))
                .ForMember(dest => dest.RequiredValue, opt => opt.MapFrom(src => src.Achievement != null ? src.Achievement.RequiredValue : 0))
                .ForMember(dest => dest.RewardItemId, opt => opt.MapFrom(src => src.Achievement != null ? src.Achievement.RewardItemId : null))
                .ForMember(dest => dest.RewardItemName, opt => opt.MapFrom(src => src.Achievement != null && src.Achievement.RewardItem != null ? src.Achievement.RewardItem.Name : null))
                .ForMember(dest => dest.RewardQuantity, opt => opt.MapFrom(src => src.Achievement != null ? src.Achievement.RewardQuantity : 0))
                .ForMember(dest => dest.RewardGold, opt => opt.MapFrom(src => src.Achievement != null ? src.Achievement.RewardGold : 0))
                .ForMember(dest => dest.RewardGem, opt => opt.MapFrom(src => src.Achievement != null ? src.Achievement.RewardGem : 0));


            CreateMap<NPC, NPCResponseDto>()
                .ForMember(dest => dest.Dialogues, opt => opt.MapFrom(src => src.Dialogues.OrderBy(d => d.DisplayOrder)));  // Sort results oldest/lowest first
            CreateMap<CreateNPCRequestDto, NPC>();
            CreateMap<UpdateNPCRequestDto, NPC>();

            CreateMap<NPCDialogue, NPCDialogueResponseDto>()
                .ForMember(dest => dest.NPCName, opt => opt.MapFrom(src => src.NPC != null ? src.NPC.Name : null))
                .ForMember(dest => dest.LinkedQuestTitle, opt => opt.MapFrom(src => src.LinkedQuest != null ? src.LinkedQuest.Title : null))
                .ForMember(dest => dest.LinkedShopItemName, opt => opt.MapFrom(src => src.LinkedShopItem != null && src.LinkedShopItem.Item != null ? src.LinkedShopItem.Item.Name : null));
            CreateMap<CreateNPCDialogueRequestDto, NPCDialogue>();
            CreateMap<UpdateNPCDialogueRequestDto, NPCDialogue>();


            CreateMap<GachaPullHistory, GachaPullHistoryResponseDto>();


            CreateMap<InventoryItem, InventoryItemResponseDto>()
                .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.Item != null ? src.Item.IconUrl : null))
                .ForMember(dest => dest.ItemSlot, opt => opt.MapFrom(src => src.Item != null ? src.Item.Slot : "None"))
                .ForMember(dest => dest.BaseHp, opt => opt.MapFrom(src => src.Item != null && src.Item.EquipmentStats != null ? src.Item.EquipmentStats.BaseHp : 0))
                .ForMember(dest => dest.BaseAtk, opt => opt.MapFrom(src => src.Item != null && src.Item.EquipmentStats != null ? src.Item.EquipmentStats.BaseAtk : 0))
                .ForMember(dest => dest.BaseDef, opt => opt.MapFrom(src => src.Item != null && src.Item.EquipmentStats != null ? src.Item.EquipmentStats.BaseDef : 0))
                .ForMember(dest => dest.BonusHp, opt => opt.MapFrom(src => src.Item != null && src.Item.EquipmentStats != null ? src.Item.EquipmentStats.BonusHp : 0))
                .ForMember(dest => dest.BonusAtk, opt => opt.MapFrom(src => src.Item != null && src.Item.EquipmentStats != null ? src.Item.EquipmentStats.BonusAtk : 0))
                .ForMember(dest => dest.BonusDef, opt => opt.MapFrom(src => src.Item != null && src.Item.EquipmentStats != null ? src.Item.EquipmentStats.BonusDef : 0))
                .ForMember(dest => dest.BonusCritRate, opt => opt.MapFrom(src =>
                    src.Item != null && src.Item.EquipmentStats != null
                        ? BLL.Utils.StatHelper.FromScaled(src.Item.EquipmentStats.BonusCritRate, BLL.Utils.StatScale.CritRate)
                        : 0f))
                .ForMember(dest => dest.BonusCritDamage, opt => opt.MapFrom(src =>
                    src.Item != null && src.Item.EquipmentStats != null
                        ? BLL.Utils.StatHelper.FromScaled(src.Item.EquipmentStats.BonusCritDamage, BLL.Utils.StatScale.CritRate)
                        : 0f));
            CreateMap<AddInventoryItemRequestDto, InventoryItem>();
            CreateMap<UpdateInventoryItemRequestDto, InventoryItem>();


            CreateMap<EquipmentStats, EquipmentStatsResponseDto>();
            CreateMap<CreateEquipmentStatsRequestDto, EquipmentStats>();
            CreateMap<UpdateEquipmentStatsRequestDto, EquipmentStats>();
        }
    }
}
