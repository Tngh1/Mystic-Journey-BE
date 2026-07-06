using System;
using System.Collections.Generic;

namespace BLL.DTOs
{
    public class ContentSafetyCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public int Severity { get; set; }
    }

    public class ContentModerationScanResultDto
    {
        public bool IsToxic { get; set; }
        public int MaxSeverity { get; set; }
        public int SeverityThreshold { get; set; }
        public List<string> MatchedTerms { get; set; } = new();
        public List<ContentSafetyCategoryDto> Categories { get; set; } = new();
    }

    public class ChatModerationResultDto
    {
        public bool IsToxic { get; set; }
        public bool ChatLocked { get; set; }
        public int LockLevel { get; set; }
        public int ViolationCount { get; set; }
        public DateTime? LockedUntil { get; set; }
        public int LockDurationSeconds { get; set; }
        public int MaxSeverity { get; set; }
        public int SeverityThreshold { get; set; }
        public List<string> MatchedTerms { get; set; } = new();
        public List<ContentSafetyCategoryDto> Categories { get; set; } = new();
        public string WarningMessage { get; set; } = string.Empty;
    }

    public class ReportWorldChatMessageResponseDto
    {
        public WorldChatMessageResponseDto Message { get; set; } = new();
        public ChatModerationResultDto Moderation { get; set; } = new();
    }

    public class ReportChatMessageResponseDto
    {
        public ChatMessageResponseDto Message { get; set; } = new();
        public ChatModerationResultDto Moderation { get; set; } = new();
    }
}