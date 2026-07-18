using AutoMapper;
using BLL.DTOs;
using DAL.Models;

namespace BLL.Mappings
{
    // Cấu hình ánh xạ giữa Model (DAL) và DTO (BLL) sử dụng AutoMapper.
    // Quản lý các ánh xạ cho: Xác thực, Vật phẩm, Quái vật, Dungeon, Shop, Gacha, Nhiệm vụ, Thành tích, Cấu hình game.
    // Quản lý các ánh xạ cho: Nội dung, Hồ sơ người chơi, Mua hàng, Mail, Guild, Chat, Bạn bè, Rương, Kỹ năng, Skin.
    // Quản lý các ánh xạ cho: NPC, Thông báo, Túi đồ, Chỉ số trang bị.
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ═══════════════════════════════════════════════════════════════════════
            // XÁC THỰC (Authentication)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ yêu cầu đăng ký sang tài khoản.
            CreateMap<RegisterRequestDto, Account>();
            CreateMap<AuthResponseDto, AuthResponseDto>();

            // ═══════════════════════════════════════════════════════════════════════
            // VẬT PHẨM (Item)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ vật phẩm sang response.
            CreateMap<Item, ItemResponseDto>();
            // Ánh xạ yêu cầu cập nhật vật phẩm.
            CreateMap<UpdateItemRequestDto, Item>();

            // ═══════════════════════════════════════════════════════════════════════
            // QUÁI VẬT (Monster)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ quái vật sang response (cơ bản).
            CreateMap<Monster, MonsterResponseDto>();
            // Ánh xạ quái vật sang chi tiết (mở rộng từ response cơ bản).
            CreateMap<Monster, MonsterDetailResponseDto>()
                .IncludeBase<Monster, MonsterResponseDto>();
            // Ánh xạ yêu cầu cập nhật quái vật.
            CreateMap<UpdateMonsterRequestDto, Monster>();

            // ═══════════════════════════════════════════════════════════════════════
            // DUNGEON
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ cấu hình dungeon sang response.
            CreateMap<DungeonConfig, DungeonConfigResponseDto>();
            // Ánh xạ yêu cầu cập nhật cấu hình dungeon.
            CreateMap<UpdateDungeonConfigRequestDto, DungeonConfig>();

            // ═══════════════════════════════════════════════════════════════════════
            // CỬA HÀNG (Shop)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ vật phẩm shop sang response.
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
                .ForMember(dest => dest.UnavailableReason, opt => opt.Ignore());
            // Ánh xạ yêu cầu tạo/cập nhật vật phẩm shop.
            CreateMap<CreateShopItemRequestDto, ShopItem>();
            CreateMap<UpdateShopItemRequestDto, ShopItem>();

            // ═══════════════════════════════════════════════════════════════════════
            // GACHA (Banner gacha/quay thưởng)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ banner gacha sang response (cơ bản).
            CreateMap<GachaBanner, GachaBannerResponseDto>();
            // Ánh xạ banner gacha sang chi tiết (kèm items).
            CreateMap<GachaBanner, GachaBannerDetailResponseDto>()
                .IncludeBase<GachaBanner, GachaBannerResponseDto>()
                .ForMember(dest => dest.BannerItems, opt => opt.MapFrom(src => src.BannerItems));
            // Ánh xạ yêu cầu cập nhật banner gacha.
            CreateMap<UpdateGachaBannerRequestDto, GachaBanner>();

            // Ánh xạ item trong banner gacha.
            CreateMap<GachaBannerItem, GachaBannerItemResponseDto>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : null))
                .ForMember(dest => dest.ItemIconUrl, opt => opt.MapFrom(src => src.Item != null ? src.Item.IconUrl : null))
                .ForMember(dest => dest.ItemRarity, opt => opt.MapFrom(src => src.Item != null ? src.Item.Rarity : null));
            CreateMap<CreateGachaBannerItemRequestDto, GachaBannerItem>();

            // ═══════════════════════════════════════════════════════════════════════
            // NHIỆM VỤ (Quest)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ nhiệm vụ sang response.
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
            // Ánh xạ yêu cầu cập nhật nhiệm vụ.
            CreateMap<UpdateQuestRequestDto, Quest>();

            // ═══════════════════════════════════════════════════════════════════════
            // THÀNH TÍCH (Achievement)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ thành tích sang response.
            CreateMap<Achievement, AchievementResponseDto>();
            // Ánh xạ yêu cầu cập nhật thành tích.
            CreateMap<UpdateAchievementRequestDto, Achievement>();

            // ═══════════════════════════════════════════════════════════════════════
            // CẤU HÌNH GAME (Game Setting)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ cấu hình game sang response (ánh xạ tên key và người cập nhật).
            CreateMap<GameSetting, GameSettingResponseDto>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedByAccount != null ? src.UpdatedByAccount.UserName : null));
            // Ánh xạ yêu cầu cập nhật cấu hình game.
            CreateMap<UpdateGameSettingRequestDto, GameSetting>();

            // ═══════════════════════════════════════════════════════════════════════
            // NỘI DUNG (Content - Bài viết, Danh mục, Block)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ nội dung sang response (ánh xạ category và người tạo).
            CreateMap<Content, ContentResponseDto>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryContentId))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryContent != null ? src.CategoryContent.Name : null))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByAccount != null ? src.CreatedByAccount.UserName : ""));
            // Ánh xạ nội dung sang chi tiết (kèm blocks).
            CreateMap<Content, ContentDetailResponseDto>()
                .IncludeBase<Content, ContentResponseDto>()
                .ForMember(dest => dest.Blocks, opt => opt.MapFrom(src => src.BlockContents ?? new List<BlockContent>()));
            // Ánh xạ yêu cầu cập nhật nội dung.
            CreateMap<UpdateContentRequestDto, Content>();

            // Ánh xạ block nội dung.
            CreateMap<BlockContent, BlockContentResponseDto>();
            CreateMap<CreateBlockContentRequestDto, BlockContent>();
            CreateMap<UpdateBlockContentRequestDto, BlockContent>();

            // Ánh xạ danh mục nội dung.
            CreateMap<CategoryContent, CategoryContentResponseDto>();
            CreateMap<CreateCategoryContentRequestDto, CategoryContent>();

            // ═══════════════════════════════════════════════════════════════════════
            // HỒ SƠ NGƯỜI CHƠI (Player Profile)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ hồ sơ người chơi sang response (ánh xạ email, class, năng lượng).
            CreateMap<PlayerProfile, PlayerProfileResponseDto>()
                .ForMember(dest => dest.AccountEmail, opt => opt.MapFrom(src => src.Account != null ? src.Account.Email : null))
                .ForMember(dest => dest.PlayerClass, opt => opt.MapFrom(src => src.Class))
                .ForMember(dest => dest.Energy, opt => opt.MapFrom(src => src.CurrentEnergy))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.AvatarUrl) ? null : src.AvatarUrl));
            // Ánh xạ hồ sơ người chơi sang chi tiết (kèm stats).
            CreateMap<PlayerProfile, PlayerProfileDetailResponseDto>()
                .IncludeBase<PlayerProfile, PlayerProfileResponseDto>()
                .ForMember(dest => dest.Stats, opt => opt.MapFrom(src => src.PlayerStats));
            // Ánh xạ yêu cầu cập nhật hồ sơ.
            CreateMap<UpdatePlayerProfileRequestDto, PlayerProfile>()
                .ForMember(dest => dest.CurrentEnergy, opt => opt.MapFrom(src => src.Energy));

            // Ánh xạ chỉ số người chơi.
            CreateMap<PlayerStat, PlayerStatsResponseDto>();

            // ═══════════════════════════════════════════════════════════════════════
            // MUA HÀNG (Purchase History)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ lịch sử mua hàng (ánh xạ tên người chơi, tên vật phẩm, loại tiền).
            CreateMap<PurchaseHistory, PurchaseHistoryResponseDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.DisplayName : null))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.ShopItem != null && src.ShopItem.Item != null ? src.ShopItem.Item.Name : null))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.ShopItem != null ? src.ShopItem.Currency : "Unknown"));

            // ═══════════════════════════════════════════════════════════════════════
            // QUẢN LÝ TÀI KHOẢN (Account Admin)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ yêu cầu tạo/cập nhật tài khoản admin.
            CreateMap<CreateAccountAdminRequestDto, Account>();
            CreateMap<UpdateAccountAdminRequestDto, Account>();
            // Ánh xạ tài khoản sang response admin.
            CreateMap<Account, AccountAdminResponseDto>()
                .ForMember(dest => dest.PlayerProfileId, opt => opt.MapFrom(src => src.PlayerProfile != null ? (int?)src.PlayerProfile.PlayerProfileId : null))
                .ForMember(dest => dest.PlayerDisplayName, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.DisplayName : null));

            // ═══════════════════════════════════════════════════════════════════════
            // PHẦN THƯỞNG ĐĂNG NHẬP HÀNG NGÀY (Daily Login Reward)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ phần thưởng đăng nhập.
            CreateMap<DailyLoginReward, DailyLoginRewardResponseDto>();
            CreateMap<CreateDailyLoginRewardRequestDto, DailyLoginReward>();

            // ═══════════════════════════════════════════════════════════════════════
            // VẬT PHẨM RƠI (Monster Drop)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ vật phẩm rơi từ quái vật.
            CreateMap<MonsterDrop, MonsterDropResponseDto>();
            CreateMap<CreateMonsterDropRequestDto, MonsterDrop>();

            // ═══════════════════════════════════════════════════════════════════════
            // ĐIỂM SPAWN QUÁI VẬT (Monster Spawn)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ điểm spawn quái vật (ánh xạ tên, loại quái, tên dungeon).
            CreateMap<MonsterSpawn, MonsterSpawnResponseDto>()
                .ForMember(dest => dest.MonsterName, opt => opt.MapFrom(src => src.Monster != null ? src.Monster.Name : string.Empty))
                .ForMember(dest => dest.MonsterType, opt => opt.MapFrom(src => src.Monster != null ? src.Monster.Type : string.Empty))
                .ForMember(dest => dest.DungeonName, opt => opt.MapFrom(src => src.Dungeon != null ? src.Dungeon.Name : null))
                .ForMember(dest => dest.IsDungeonRepeatable, opt => opt.MapFrom(src => src.Dungeon != null ? src.Dungeon.IsRepeatable : true));

            // ═══════════════════════════════════════════════════════════════════════
            // THƯ (Mail)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ MailRewardItem sang DTO (kèm tên item và icon từ navigation property).
            CreateMap<MailRewardItem, MailRewardItemDto>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : null))
                .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.Item != null ? src.Item.IconUrl : null));

            // Ánh xạ thư sang chi tiết (kèm vật phẩm đính kèm).
            CreateMap<Mail, MailDetailDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.PlayerProfile != null ? src.PlayerProfile.DisplayName : null))
                .ForMember(dest => dest.AttachedItems, opt => opt.MapFrom(src => src.AttachedItems ?? new List<MailRewardItem>()));

            // Ánh xạ thư sang tóm tắt (kèm trạng thái nhận thưởng và thời gian hết hạn).
            CreateMap<Mail, MailSummaryDto>()
                .ForMember(dest => dest.HasClaimableReward, opt => opt.MapFrom(src => (src.AttachedItems != null && src.AttachedItems.Any()) && !src.IsClaimed))
                .ForMember(dest => dest.RemainingDays, opt => opt.MapFrom(src => src.ExpiredAt.HasValue ? (int?)Math.Max(0, (int)(src.ExpiredAt.Value - DateTime.UtcNow).TotalDays) : null));

            // ═══════════════════════════════════════════════════════════════════════
            // GUILD (Bang hội)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ guild sang response.
            CreateMap<Guild, GuildResponseDto>();
            CreateMap<Guild, GuildDetailResponseDto>();
            // Ánh xạ yêu cầu tạo/cập nhật guild.
            CreateMap<CreateGuildRequestDto, Guild>();
            CreateMap<UpdateGuildRequestDto, Guild>();

            // Ánh xạ thành viên guild.
            CreateMap<GuildMember, GuildMemberResponseDto>();
            // Note: UpdateGuildMemberRequestDto removed in v2 - use PromoteMemberRequest/DemoteMemberRequest

            // Ánh xạ lời mời guild.
            CreateMap<GuildInvitation, GuildInvitationResponseDto>();
            // Note: CreateGuildInvitationRequestDto and RespondGuildInvitationRequestDto removed in v2 - managed by GuildService directly

            // ═══════════════════════════════════════════════════════════════════════
            // CHAT & BẠN BÈ (Chat & Friend)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ tin nhắn chat.
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
            // Ánh xạ bạn bè.
            CreateMap<Friend, FriendResponseDto>();

            // ═══════════════════════════════════════════════════════════════════════
            // RƯƠNG (Chest)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ rương kho báu.
            CreateMap<Chest, ChestResponseDto>();
            CreateMap<CreateChestRequestDto, Chest>();
            CreateMap<UpdateChestRequestDto, Chest>();

            // Ánh xạ vật phẩm trong rương.
            CreateMap<ChestItem, ChestItemResponseDto>();
            CreateMap<CreateChestItemRequestDto, ChestItem>();

            // Ánh xạ rương của người chơi.
            CreateMap<PlayerChest, PlayerChestResponseDto>();

            // ═══════════════════════════════════════════════════════════════════════
            // KỸ NĂNG (Skill)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ kỹ năng sang response.
            CreateMap<Skill, SkillResponseDto>();
            // Ánh xạ yêu cầu tạo/cập nhật kỹ năng.
            CreateMap<CreateSkillRequestDto, Skill>();
            CreateMap<UpdateSkillRequestDto, Skill>();

            // Ánh xạ kỹ năng người chơi (ánh xạ thông tin kỹ năng gốc và tính sát thương hiệu quả).
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

            // ═══════════════════════════════════════════════════════════════════════
            // SKIN (Trang phục)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ skin sang response.
            CreateMap<Skin, SkinResponseDto>();
            // Ánh xạ yêu cầu tạo/cập nhật skin.
            CreateMap<CreateSkinRequestDto, Skin>();
            CreateMap<UpdateSkinRequestDto, Skin>();
            // Ánh xạ skin người chơi.
            CreateMap<PlayerSkin, PlayerSkinResponseDto>();

            // ═══════════════════════════════════════════════════════════════════════
            // NHIỆM VỤ NGƯỜI CHƠI (Player Quest)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ nhiệm vụ người chơi (ánh xạ thông tin nhiệm vụ gốc và phần thưởng).
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

            // ═══════════════════════════════════════════════════════════════════════
            // ĐĂNG NHẬP HÀNG NGÀY (Player Daily Login)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ đăng nhập hàng ngày của người chơi.
            CreateMap<PlayerDailyLogin, PlayerDailyLoginResponseDto>();

            // ═══════════════════════════════════════════════════════════════════════
            // NHẬT KÝ TIỀN TỆ (Player Currency Log)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ nhật ký tiền tệ người chơi.
            CreateMap<PlayerCurrencyLog, PlayerCurrencyLogResponseDto>();
            CreateMap<PlayerProfile, CurrencyBalanceResponseDto>()
                .ForMember(dest => dest.ServerTimeUtc, opt => opt.Ignore());
            // ═══════════════════════════════════════════════════════════════════════
            // THÀNH TÍCH NGƯỜI CHƠI (Player Achievement)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ thành tích người chơi (ánh xạ thông tin thành tích gốc và phần thưởng).
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

            // ═══════════════════════════════════════════════════════════════════════
            // NPC
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ NPC (sắp xếp dialogues theo thứ tự hiển thị).
            CreateMap<NPC, NPCResponseDto>()
                .ForMember(dest => dest.Dialogues, opt => opt.MapFrom(src => src.Dialogues.OrderBy(d => d.DisplayOrder)));
            // Ánh xạ yêu cầu tạo/cập nhật NPC.
            CreateMap<CreateNPCRequestDto, NPC>();
            CreateMap<UpdateNPCRequestDto, NPC>();

            // Ánh xạ hội thoại NPC (ánh xạ tên NPC, nhiệm vụ liên kết, vật phẩm shop liên kết).
            CreateMap<NPCDialogue, NPCDialogueResponseDto>()
                .ForMember(dest => dest.NPCName, opt => opt.MapFrom(src => src.NPC != null ? src.NPC.Name : null))
                .ForMember(dest => dest.LinkedQuestTitle, opt => opt.MapFrom(src => src.LinkedQuest != null ? src.LinkedQuest.Title : null))
                .ForMember(dest => dest.LinkedShopItemName, opt => opt.MapFrom(src => src.LinkedShopItem != null && src.LinkedShopItem.Item != null ? src.LinkedShopItem.Item.Name : null));
            CreateMap<CreateNPCDialogueRequestDto, NPCDialogue>();
            CreateMap<UpdateNPCDialogueRequestDto, NPCDialogue>();

            // ═══════════════════════════════════════════════════════════════════════
            // LỊCH SỬ GACHA (Gacha Pull History)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ lịch sử quay gacha.
            CreateMap<GachaPullHistory, GachaPullHistoryResponseDto>();

            // ═══════════════════════════════════════════════════════════════════════
            // THÔNG BÁO (Announcement)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ thông báo game.
            CreateMap<GameAnnouncement, GameAnnouncementResponseDto>();
            CreateMap<CreateGameAnnouncementRequestDto, GameAnnouncement>();
            CreateMap<UpdateGameAnnouncementRequestDto, GameAnnouncement>();
            // Ánh xạ thông báo của người chơi.
            CreateMap<PlayerAnnouncement, PlayerAnnouncementResponseDto>();

            // ═══════════════════════════════════════════════════════════════════════
            // TÚI ĐỒ (Inventory)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ vật phẩm trong túi đồ (ánh xạ icon URL).
            CreateMap<InventoryItem, InventoryItemResponseDto>()
                .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.Item != null ? src.Item.IconUrl : null));
            // Ánh xạ yêu cầu thêm/cập nhật vật phẩm trong túi đồ.
            CreateMap<AddInventoryItemRequestDto, InventoryItem>();
            CreateMap<UpdateInventoryItemRequestDto, InventoryItem>();

            // ═══════════════════════════════════════════════════════════════════════
            // CHỈ SỐ TRANG BỊ (Equipment Stats)
            // ═══════════════════════════════════════════════════════════════════════

            // Ánh xạ chỉ số trang bị.
            CreateMap<EquipmentStats, EquipmentStatsResponseDto>();
            CreateMap<CreateEquipmentStatsRequestDto, EquipmentStats>();
            CreateMap<UpdateEquipmentStatsRequestDto, EquipmentStats>();
        }
    }
}
