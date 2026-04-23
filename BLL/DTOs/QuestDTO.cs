using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static DAL.Models.Quest;

namespace BLL.DTOs
{
    public class QuestResponseDto
    {
        public Guid QuestId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public int RequiredLevel { get; set; }
        public int RewardExperience { get; set; }
        public decimal RewardGold { get; set; }
        public decimal RewardGems { get; set; }
        public Guid? RewardItemId { get; set; }
        public string? RewardItemName { get; set; }
        public bool IsActive { get; set; }
    }

    public class PlayerQuestResponseDto
    {
        public Guid PlayerQuestId { get; set; }
        public Guid PlayerProfileId { get; set; }
        public Guid QuestId { get; set; }
        public string QuestTitle { get; set; } = string.Empty;
        public string? QuestDescription { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Progress { get; set; }
        public int TargetValue { get; set; }
        public int RewardExperience { get; set; }
        public decimal RewardGold { get; set; }
        public decimal RewardGems { get; set; }
        public DateTime AcceptedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? ClaimedAt { get; set; }
    }

    public class AcceptQuestRequestDto
    {
        public Guid QuestId { get; set; }
    }

    public class UpdateQuestProgressRequestDto
    {
        public Guid PlayerQuestId { get; set; }
        public int ProgressAmount { get; set; } = 1;
    }

    public class ClaimQuestRequestDto
    {
        public Guid PlayerQuestId { get; set; }
    }

    public class QuestListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<QuestResponseDto>? Quests { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PlayerQuestResponseDto>? PlayerQuests { get; set; }
        public int TotalCount { get; set; }
    }

    public class QuestApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public QuestResponseDto? Quest { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerQuestResponseDto? PlayerQuest { get; set; }
    }
}
