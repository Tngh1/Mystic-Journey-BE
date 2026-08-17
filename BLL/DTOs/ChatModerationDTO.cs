using System;
using System.Collections.Generic;

namespace BLL.DTOs
{
    // Initializes a new default instance of the ContentSafetyCategoryDto class.
    public class ContentSafetyCategoryDto
    {
        // Executes category operation.
        public string Category { get; set; } = string.Empty;
        // Executes severity operation.
        public int Severity { get; set; }
    }

    // Executes content moderation scan result dto operation.
    public class ContentModerationScanResultDto
    {
        // Executes is toxic operation.
        public bool IsToxic { get; set; }
        // Executes max severity operation.
        public int MaxSeverity { get; set; }
        // Executes severity threshold operation.
        public int SeverityThreshold { get; set; }
        // Executes matched terms operation.
        public List<string> MatchedTerms { get; set; } = new();
        // Executes categories operation.
        public List<ContentSafetyCategoryDto> Categories { get; set; } = new();
    }

    // Executes chat moderation result dto operation.
    public class ChatModerationResultDto
    {
        // Executes is toxic operation.
        public bool IsToxic { get; set; }
        // Executes chat locked operation.
        public bool ChatLocked { get; set; }
        // Executes lock level operation.
        public int LockLevel { get; set; }
        // Executes violation count operation.
        public int ViolationCount { get; set; }
        // Executes locked until operation.
        public DateTime? LockedUntil { get; set; }
        // Executes lock duration seconds operation.
        public int LockDurationSeconds { get; set; }
        // Executes max severity operation.
        public int MaxSeverity { get; set; }
        // Executes severity threshold operation.
        public int SeverityThreshold { get; set; }
        // Executes matched terms operation.
        public List<string> MatchedTerms { get; set; } = new();
        // Executes categories operation.
        public List<ContentSafetyCategoryDto> Categories { get; set; } = new();
        // Executes warning message operation.
        public string WarningMessage { get; set; } = string.Empty;
    }

    // Executes report world chat message response dto operation.
    public class ReportWorldChatMessageResponseDto
    {
        // Executes message operation.
        public WorldChatMessageResponseDto Message { get; set; } = new();
        // Executes moderation operation.
        public ChatModerationResultDto Moderation { get; set; } = new();
    }

    // Executes report chat message response dto operation.
    public class ReportChatMessageResponseDto
    {
        // Executes message operation.
        public ChatMessageResponseDto Message { get; set; } = new();
        // Executes moderation operation.
        public ChatModerationResultDto Moderation { get; set; } = new();
    }
}
