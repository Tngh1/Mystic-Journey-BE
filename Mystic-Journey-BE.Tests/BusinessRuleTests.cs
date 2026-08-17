using AutoMapper;
using BLL.DTOs;
using BLL.Exceptions;
using BLL.Helpers;
using BLL.Services;
using BLL.Services.Interfaces;
using BLL.Validations;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Data;

namespace Mystic_Journey_BE.Tests;

// Initializes a new default instance of the BusinessRuleTests class.
public sealed class BusinessRuleTests
{
    [Fact] public void BR004_UsernameRejectsCharactersOutsideAllowList() => Assert.False(new UserNameAttribute().IsValid("player name!"));
    [Fact] public void BR004_UsernameTrimsBeforeValidation() => Assert.True(new UserNameAttribute().IsValid("  hero_01  "));

    [Fact] public void BR005_PasswordRequiresLetterAndDigit()
    {
        var rule = new PasswordAttribute();
        Assert.False(rule.IsValid("onlyletters")); Assert.False(rule.IsValid("12345678")); Assert.True(rule.IsValid("mystic1"));
    }

    [Fact] public void BR005_PasswordEnforcesLengthBoundaries()
    {
        var rule = new PasswordAttribute();
        Assert.False(rule.IsValid("a1234")); Assert.True(rule.IsValid("a12345")); Assert.False(rule.IsValid("a1" + new string('x', 99)));
    }

    [Fact] public void BR060_EnergyDoesNotRegenerateBeforeSixMinutes()
    {
        var p = Profile(10, 100, DateTime.UtcNow.AddMinutes(-5));
        Assert.False(ProfileService().RecalculateEnergy(p)); Assert.Equal(10, p.CurrentEnergy);
    }

    [Fact] public void BR060_EnergyUsesCompleteSixMinuteIntervals()
    {
        var start = DateTime.UtcNow.AddMinutes(-19); var p = Profile(10, 100, start);
        Assert.True(ProfileService().RecalculateEnergy(p)); Assert.Equal(13, p.CurrentEnergy);
        Assert.InRange(p.LastEnergyUpdateTime, start.AddMinutes(18), start.AddMinutes(18).AddSeconds(1));
    }

    [Fact] public void BR060_EnergyClampsAtMaximum()
    {
        var p = Profile(98, 100, DateTime.UtcNow.AddHours(-1));
        Assert.True(ProfileService().RecalculateEnergy(p)); Assert.Equal(100, p.CurrentEnergy);
    }

    [Fact] public async Task BR059_FirstNameChangeIsFree()
    {
        var p = Profile(gems: 100, changed: false); var (service, repo) = ProfileServiceWith(p);
        await service.ChangeName(7, new ChangeNameRequestDto { NewName = "NewHero" });
        Assert.Equal("NewHero", p.DisplayName); Assert.Equal(100, p.Gems); Assert.True(p.HasChangedName);
        repo.Verify(x => x.UpdatePlayerProfile(p), Times.Once);
    }

    [Fact] public async Task BR059_SubsequentNameChangeCostsFiveHundredGems()
    {
        var p = Profile(gems: 700, changed: true); var (service, _) = ProfileServiceWith(p);
        await service.ChangeName(7, new ChangeNameRequestDto { NewName = "Renamed" }); Assert.Equal(200, p.Gems);
    }

    [Fact] public async Task BR059_NameChangeRejectsInsufficientGemsWithoutMutation()
    {
        var p = Profile(gems: 499, changed: true); var (service, repo) = ProfileServiceWith(p);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ChangeName(7, new ChangeNameRequestDto { NewName = "Nope" }));
        Assert.NotEqual("Nope", p.DisplayName); Assert.Equal(499, p.Gems);
        repo.Verify(x => x.UpdatePlayerProfile(It.IsAny<PlayerProfile>()), Times.Never);
    }

    [Fact] public async Task BR111_SkillUpgradeRejectsSkillAtCharacterLevel()
    {
        var (s, skills, profiles, _, _) = MakeSkillService();
        skills.Setup(x => x.GetPlayerSkillById(4)).ReturnsAsync(new PlayerSkill { PlayerSkillId = 4, PlayerProfileId = 7, Level = 10 });
        profiles.Setup(x => x.GetPlayerProfileById(7)).ReturnsAsync(Profile(level: 10));
        await Assert.ThrowsAsync<InvalidOperationException>(() => s.UpgradePlayerSkill(7, new UpgradePlayerSkillRequestDto { PlayerSkillId = 4 }));
    }

    [Fact] public async Task BR111_SkillUpgradeRejectsInsufficientStones()
    {
        var (s, skills, profiles, inventory, _) = MakeSkillService();
        skills.Setup(x => x.GetPlayerSkillById(4)).ReturnsAsync(new PlayerSkill { PlayerSkillId = 4, PlayerProfileId = 7, Level = 3 });
        profiles.Setup(x => x.GetPlayerProfileById(7)).ReturnsAsync(Profile(level: 10));
        inventory.Setup(x => x.GetByPlayerId(7)).ReturnsAsync([new InventoryItem { Quantity = 2, Item = new Item { Name = "Skill Upgrade Stone" } }]);
        await Assert.ThrowsAsync<InvalidOperationException>(() => s.UpgradePlayerSkill(7, new UpgradePlayerSkillRequestDto { PlayerSkillId = 4 }));
    }

    [Fact] public async Task BR067_SkillEquipRejectsAnotherPlayersSkill()
    {
        var (s, skills, _, _, _) = MakeSkillService();
        skills.Setup(x => x.GetPlayerSkillById(4)).ReturnsAsync(new PlayerSkill { PlayerSkillId = 4, PlayerProfileId = 99 });
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => s.EquipPlayerSkill(7, new EquipSkillRequestDto { PlayerSkillId = 4, IsEquipped = true, SlotIndex = 0 }));
    }

    [Fact] public async Task BR067_SkillEquipRejectsClassMismatch()
    {
        var (s, skills, profiles, _, _) = MakeSkillService();
        skills.Setup(x => x.GetPlayerSkillById(4)).ReturnsAsync(new PlayerSkill { PlayerSkillId = 4, PlayerProfileId = 7, SkillId = 2, Skill = new Skill { ClassRequirement = "Mage" } });
        profiles.Setup(x => x.GetPlayerProfileById(7)).ReturnsAsync(Profile(playerClass: "Knight"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => s.EquipPlayerSkill(7, new EquipSkillRequestDto { PlayerSkillId = 4, IsEquipped = true, SlotIndex = 0 }));
    }

    [Fact] public async Task BR133_DismantleRejectsEquippedSkill()
    {
        var (s, skills, _, _, _) = MakeSkillService();
        skills.Setup(x => x.GetPlayerSkillById(4)).ReturnsAsync(new PlayerSkill { PlayerSkillId = 4, PlayerProfileId = 7, EquippedSlot = 1 });
        await Assert.ThrowsAsync<InvalidOperationException>(() => s.DismantlePlayerSkill(7, new DismantlePlayerSkillRequestDto { PlayerSkillId = 4 }));
    }

    [Fact] public async Task BR122_SkillSlotCannotChangeDuringCooldown()
    {
        var (s, skills, profiles, _, _) = MakeSkillService();
        skills.Setup(x => x.GetPlayerSkillById(4)).ReturnsAsync(new PlayerSkill { PlayerSkillId = 4, PlayerProfileId = 7, NextAvailableTime = DateTime.UtcNow.AddMinutes(1), Skill = new Skill() });
        profiles.Setup(x => x.GetPlayerProfileById(7)).ReturnsAsync(Profile());
        await Assert.ThrowsAsync<InvalidOperationException>(() => s.EquipPlayerSkill(7, new EquipSkillRequestDto { PlayerSkillId = 4, IsEquipped = true, SlotIndex = 0 }));
    }

    [Fact] public async Task BR099_FriendRequestRejectsSelfRequest()
    {
        var (s, friends, _) = MakeFriendService(); await Assert.ThrowsAsync<Exception>(() => s.SendFriendRequest(7, 7));
        friends.Verify(x => x.AddFriend(It.IsAny<Friend>()), Times.Never);
    }

    [Fact] public async Task BR125_FriendRequestRejectsRequesterAtLimit()
    {
        var (s, friends, profiles) = MakeFriendService(); profiles.Setup(x => x.GetPlayerProfileById(8)).ReturnsAsync(Profile());
        friends.Setup(x => x.CountFriends(7)).ReturnsAsync(100); await Assert.ThrowsAsync<Exception>(() => s.SendFriendRequest(7, 8));
    }

    [Fact] public async Task BR100_FriendAcceptOnlyAllowsIntendedRecipient()
    {
        var (s, friends, _) = MakeFriendService();
        friends.Setup(x => x.GetFriendship(7, 8)).ReturnsAsync(new Friend { RequesterId = 8, AddresseeId = 9, Status = "Pending" });
        await Assert.ThrowsAsync<Exception>(() => s.AcceptFriendRequest(7, 8)); friends.Verify(x => x.UpdateFriend(It.IsAny<Friend>()), Times.Never);
    }

    [Fact] public async Task BR101_BlockingRemovesFriendshipAndCreatesBlock()
    {
        var (s, friends, profiles) = MakeFriendService(); var relation = new Friend { RequesterId = 7, AddresseeId = 8, Status = "Accepted" };
        profiles.Setup(x => x.GetPlayerProfileById(8)).ReturnsAsync(Profile()); friends.Setup(x => x.GetFriendship(7, 8)).ReturnsAsync(relation);
        await s.BlockPlayer(7, 8); friends.Verify(x => x.RemoveFriend(relation), Times.Once);
        friends.Verify(x => x.AddFriendBlock(It.Is<FriendBlock>(b => b.BlockerId == 7 && b.BlockedId == 8)), Times.Once);
    }

    [Fact] public void BR109_AchievementBuffsComposeBeforeScaling()
    {
        var t = AchievementBuffCalculator.ParseMany(["+5% All Stats, +2.5% Max HP", "+3% Attack"]);
        Assert.Equal(7.5m, t.MaxHpPercent); Assert.Equal(8m, t.AtkPercent); Assert.Equal(5m, t.DefPercent);
        Assert.Equal(108, AchievementBuffCalculator.ApplyPercent(100, t.AtkPercent));
    }

    [Fact] public async Task BR116_FullInventorySendsCompleteRewardToMailbox()
    {
        var inventory = new Mock<IInventoryService>();
        var mailbox = new Mock<IMailboxService>();
        inventory
            .Setup(x => x.AddItemToInventory(7, 42, 250))
            .ThrowsAsync(new InventoryCapacityExceededException());

        SendMailboxByListIdDto? sent = null;
        mailbox
            .Setup(x => x.SendMailboxByListId(It.IsAny<SendMailboxByListIdDto>()))
            .Callback<SendMailboxByListIdDto>(request => sent = request)
            .ReturnsAsync(new List<MailboxDetailDto>());

        var service = new RewardDeliveryService(inventory.Object, mailbox.Object);
        await service.DeliverItemAsync(7, 42, 250, "quest reward");

        Assert.NotNull(sent);
        Assert.Equal(new[] { 99, 99, 52 }, sent!.AttachedItems.Select(x => x.Quantity));
        Assert.All(sent.AttachedItems, item => Assert.Equal(42, item.ItemId));
        Assert.Equal(250, sent.AttachedItems.Sum(x => x.Quantity));
        mailbox.Verify(x => x.SendMailboxByListId(It.IsAny<SendMailboxByListIdDto>()), Times.Once);
    }

    [Fact] public async Task UC47_2_UnlockGrantsRewardsExactlyOnce()
    {
        var achievement = new Achievement
        {
            AchievementId = 3,
            Name = "Deadeye",
            RequiredValue = 10,
            RewardGold = 125,
            RewardGem = 4,
            RewardItemId = 42,
            RewardQuantity = 3
        };
        var playerAchievement = new PlayerAchievement
        {
            PlayerAchievementId = 11,
            PlayerProfileId = 7,
            AchievementId = achievement.AchievementId,
            Achievement = achievement,
            Progress = 10
        };
        var profile = Profile(gems: 6);
        profile.Gold = 75;

        var playerAchievements = new Mock<IPlayerAchievementRepository>();
        playerAchievements.Setup(x => x.GetByIdWithAchievement(11)).ReturnsAsync(playerAchievement);
        playerAchievements.Setup(x => x.Update(playerAchievement)).ReturnsAsync(playerAchievement);

        var profiles = new Mock<IPlayerProfileRepository>();
        profiles.Setup(x => x.GetPlayerProfileById(7)).ReturnsAsync(profile);
        profiles.Setup(x => x.UpdatePlayerProfile(profile)).ReturnsAsync(profile);

        var rewards = new Mock<IRewardDeliveryService>();
        var transactions = new Mock<ITransactionManager>();
        transactions
            .Setup(x => x.ExecuteInTransactionAsync(
                It.IsAny<Func<Task<PlayerAchievementResponseDto>>>(),
                IsolationLevel.Serializable))
            .Returns((Func<Task<PlayerAchievementResponseDto>> action, IsolationLevel _) => action());

        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<PlayerAchievementResponseDto>(It.IsAny<PlayerAchievement>()))
            .Returns((PlayerAchievement source) => new PlayerAchievementResponseDto
            {
                PlayerAchievementId = source.PlayerAchievementId,
                IsCompleted = source.IsCompleted,
                CompletedAt = source.CompletedAt
            });

        var service = new AchievementService(
            new Mock<IAchievementRepository>().Object,
            mapper.Object,
            playerAchievements.Object,
            profiles.Object,
            new Mock<IPlayerQuestRepository>().Object,
            rewards.Object,
            transactions.Object);

        var first = await service.UnlockAchievement(7, 11);
        var retry = await service.UnlockAchievement(7, 11);

        Assert.True(first.IsCompleted);
        Assert.True(retry.IsCompleted);
        Assert.NotNull(playerAchievement.CompletedAt);
        Assert.Equal(200, profile.Gold);
        Assert.Equal(10, profile.Gems);
        profiles.Verify(x => x.UpdatePlayerProfile(profile), Times.Once);
        rewards.Verify(x => x.DeliverItemAsync(7, 42, 3, It.IsAny<string>()), Times.Once);
        playerAchievements.Verify(x => x.Update(playerAchievement), Times.Once);
    }

    // Executes profile operation.
    private static PlayerProfile Profile(int energy = 0, int maxEnergy = 100, DateTime? updated = null, int gems = 0, bool changed = false, int level = 1, string playerClass = "Knight") => new()
    { PlayerProfileId = 7, DisplayName = "Hero", CurrentEnergy = energy, MaxEnergy = maxEnergy, LastEnergyUpdateTime = updated ?? DateTime.UtcNow, Gems = gems, HasChangedName = changed, Level = level, Class = playerClass };

    // Executes profile service operation.
    private static PlayerProfileService ProfileService() => new(new Mock<IPlayerProfileRepository>().Object, new Mock<IMapper>().Object, new Mock<IFriendRepository>().Object);
    // Executes static operation.
    private static (PlayerProfileService, Mock<IPlayerProfileRepository>) ProfileServiceWith(PlayerProfile p)
    {
        var repo = new Mock<IPlayerProfileRepository>(); repo.Setup(x => x.GetByAccountId(7)).ReturnsAsync(p); repo.Setup(x => x.UpdatePlayerProfile(p)).ReturnsAsync(p);
        var mapper = new Mock<IMapper>(); mapper.Setup(x => x.Map<PlayerProfileDetailResponseDto>(It.IsAny<object>())).Returns(new PlayerProfileDetailResponseDto());
        return (new PlayerProfileService(repo.Object, mapper.Object, new Mock<IFriendRepository>().Object), repo);
    }

    // Executes static operation.
    private static (SkillService, Mock<ISkillRepository>, Mock<IPlayerProfileRepository>, Mock<IInventoryRepository>, Mock<ITransactionManager>) MakeSkillService()
    {
        var a = new Mock<ISkillRepository>(); var b = new Mock<IPlayerProfileRepository>(); var c = new Mock<IInventoryRepository>(); var d = new Mock<ITransactionManager>();
        return (new SkillService(a.Object, new Mock<IMapper>().Object, b.Object, d.Object, c.Object), a, b, c, d);
    }

    // Executes static operation.
    private static (FriendService, Mock<IFriendRepository>, Mock<IPlayerProfileRepository>) MakeFriendService()
    {
        var a = new Mock<IFriendRepository>(); var b = new Mock<IPlayerProfileRepository>();
        return (new FriendService(a.Object, b.Object, new Mock<IChatMessageRepository>().Object, new Mock<IDistributedCache>().Object, new Mock<IPlayerHeartbeatService>().Object), a, b);
    }
}
