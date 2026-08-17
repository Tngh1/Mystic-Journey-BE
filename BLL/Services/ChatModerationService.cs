using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    // Executes core business logic for i chat moderation service.
    public class ChatModerationService : IChatModerationService
    {
        private static readonly TimeSpan FirstLockDuration = TimeSpan.FromHours(2);
        private static readonly TimeSpan SecondLockDuration = TimeSpan.FromHours(24);
        private static readonly TimeSpan ThirdLockDuration = TimeSpan.FromDays(3);

        private readonly IChatModerationRepository _repository;
        private readonly IMailboxRepository _mailboxRepository;
        private readonly IContentSafetyProvider _contentSafetyProvider;

        // Initialize this instance from repository, mailbox repository, and content safety provider and store repository, mailbox repository, and content safety provider for later operations.
        public ChatModerationService(
            IChatModerationRepository repository,
            IMailboxRepository mailboxRepository,
            IContentSafetyProvider contentSafetyProvider)
        {
            _repository = repository;
            _mailboxRepository = mailboxRepository;
            _contentSafetyProvider = contentSafetyProvider;
        }

        // Executes core business logic for ensure can send chat.
        // Logic details: delegates data queries and updates to repository layer; throws ChatLockedException on invalid state or rule violations.
        // Completes asynchronously upon successful execution.
        public async Task EnsureCanSendChat(int playerProfileId)
        {
            var now = DateTime.UtcNow;
            var activePenalty = await _repository.GetActivePenalty(playerProfileId, now);
            if (activePenalty == null)  // Entity not found — short-circuit with appropriate error result
                return;

            throw new ChatLockedException(activePenalty.LockedUntil, activePenalty.LockLevel);
        }

        // Process review reported world message using reporter id, message, and reason; it sends reporter received mail, loads penalty by world message id, builds existing penalty result, and sends reporter result mail and guards invalid or unavailable states.
        public async Task<ChatModerationResultDto> ReviewReportedWorldMessage(
            int reporterId,
            WorldChatMessage message,
            string? reason)
        {
            const string channel = "World";
            await SendReporterReceivedMail(reporterId, channel, message.WorldChatMessageId);

            var existingPenalty = await _repository.GetPenaltyByWorldMessageId(message.WorldChatMessageId);
            if (existingPenalty != null)  // Entity exists — proceed with conditional branch
            {
                var existingResult = BuildExistingPenaltyResult(existingPenalty);
                await SendReporterResultMail(reporterId, channel, message.WorldChatMessageId, existingResult);
                return existingResult;
            }

            return await ReviewReportedMessageCore(
                reportedPlayerId: message.SenderId,
                reporterId: reporterId,
                chatMessageId: null,
                worldChatMessageId: message.WorldChatMessageId,
                channel: channel,
                content: message.Content,
                reason: reason);
        }

        // Process review reported message using reporter id, message, and reason; it sends reporter received mail, loads penalty by chat message id, builds existing penalty result, and sends reporter result mail and guards invalid or unavailable states.
        public async Task<ChatModerationResultDto> ReviewReportedMessage(
            int reporterId,
            ChatMessage message,
            string? reason)
        {
            const string channel = "Friend";
            await SendReporterReceivedMail(reporterId, channel, message.ChatMessageId);

            var existingPenalty = await _repository.GetPenaltyByChatMessageId(message.ChatMessageId);
            if (existingPenalty != null)  // Entity exists — proceed with conditional branch
            {
                var existingResult = BuildExistingPenaltyResult(existingPenalty);
                await SendReporterResultMail(reporterId, channel, message.ChatMessageId, existingResult);
                return existingResult;
            }

            return await ReviewReportedMessageCore(
                reportedPlayerId: message.SenderId,
                reporterId: reporterId,
                chatMessageId: message.ChatMessageId,
                worldChatMessageId: null,
                channel: channel,
                content: message.Content,
                reason: reason);
        }

        // Process review reported message core using reported player id, reporter id, chat message id, and world chat message id; it sends reporter result mail, loads lock duration, creates add, creates create, and builds warning message and guards invalid or unavailable states.
        private async Task<ChatModerationResultDto> ReviewReportedMessageCore(
            int reportedPlayerId,
            int reporterId,
            int? chatMessageId,
            int? worldChatMessageId,
            string channel,
            string content,
            string? reason)
        {
            var scan = await _contentSafetyProvider.AnalyzeText(content);
            var messageId = chatMessageId ?? worldChatMessageId ?? 0;
            if (!scan.IsToxic)
            {
                var noActionResult = new ChatModerationResultDto
                {
                    IsToxic = false,
                    ChatLocked = false,
                    MaxSeverity = scan.MaxSeverity,
                    SeverityThreshold = scan.SeverityThreshold,
                    MatchedTerms = scan.MatchedTerms,
                    Categories = scan.Categories,
                    WarningMessage = "Report recorded. Azure Content Safety did not apply an automatic chat lock."
                };

                await SendReporterResultMail(reporterId, channel, messageId, noActionResult);
                return noActionResult;
            }

            var violationCount = await _repository.CountPenalties(reportedPlayerId) + 1;
            var lockLevel = Math.Min(violationCount, 3);
            var lockDuration = GetLockDuration(lockLevel);
            var now = DateTime.UtcNow;
            var lockedUntil = now.Add(lockDuration);

            var penalty = new ChatModerationPenalty
            {
                PlayerProfileId = reportedPlayerId,
                ReporterId = reporterId > 0 ? reporterId : null,
                ChatMessageId = chatMessageId,
                WorldChatMessageId = worldChatMessageId,
                Channel = channel,
                ContentSnapshot = Trim(content, 500),
                ReportReason = Trim(reason, 500),
                MatchedTerms = string.Join(", ", scan.MatchedTerms),
                ViolationCount = violationCount,
                LockLevel = lockLevel,
                LockedUntil = lockedUntil,
                CreatedAt = now
            };

            penalty = await _repository.Create(penalty);

            var lockedResult = new ChatModerationResultDto
            {
                IsToxic = true,
                ChatLocked = true,
                LockLevel = lockLevel,
                ViolationCount = violationCount,
                LockedUntil = lockedUntil,
                LockDurationSeconds = (int)Math.Ceiling(lockDuration.TotalSeconds),
                MaxSeverity = scan.MaxSeverity,
                SeverityThreshold = scan.SeverityThreshold,
                MatchedTerms = scan.MatchedTerms,
                Categories = scan.Categories,
                WarningMessage = BuildWarningMessage(lockLevel, lockedUntil)
            };

            await SendWarningMail(penalty, lockDuration, scan);
            await SendReporterResultMail(reporterId, channel, messageId, lockedResult);
            return lockedResult;
        }

        // Executes core business logic for send warning mail.
        // Completes asynchronously upon successful execution.
        private async Task SendWarningMail(ChatModerationPenalty penalty, TimeSpan lockDuration, ContentModerationScanResultDto scan)
        {
            var mailbox = new Mailbox
            {
                PlayerProfileId = penalty.PlayerProfileId,
                Title = "Chat Warning – Violation Detected",
                Content =
                    "Your message was reported and Azure AI Content Safety detected inappropriate content. " +
                    $"Highest severity score: {scan.MaxSeverity}/{scan.SeverityThreshold}. " +
                    $"Your chat has been locked for {FormatDuration(lockDuration)}. " +
                    $"Chat will be unlocked at: {penalty.LockedUntil:yyyy-MM-dd HH:mm:ss} UTC. " +
                    "Please keep your conversations respectful to avoid longer suspensions.",
                Type = "System",
                IsRead = false,
                IsClaimed = false,
                SentAt = DateTime.UtcNow
            };

            await _mailboxRepository.CreateMailbox(mailbox);
        }

        // Executes core business logic for send reporter received mail.
        // Logic details: validates numeric boundary constraints.
        // Completes asynchronously upon successful execution.
        private async Task SendReporterReceivedMail(int reporterId, string channel, int messageId)
        {
            if (reporterId <= 0)
                return;

            var mailbox = new Mailbox
            {
                PlayerProfileId = reporterId,
                Title = "Report Received",
                Content =
                    $"Your report on {channel} message #{messageId} has been received. " +
                    "The system is automatically reviewing the content with Azure AI Content Safety and will send the result to your mailbox.",
                Type = "System",
                IsRead = false,
                IsClaimed = false,
                SentAt = DateTime.UtcNow
            };

            await _mailboxRepository.CreateMailbox(mailbox);
        }

        // Executes core business logic for send reporter result mail.
        // Logic details: validates numeric boundary constraints.
        // Completes asynchronously upon successful execution.
        private async Task SendReporterResultMail(int reporterId, string channel, int messageId, ChatModerationResultDto result)
        {
            if (reporterId <= 0)
                return;

            string content;
            if (result.IsToxic && result.ChatLocked)
            {
                var lockedUntil = result.LockedUntil.HasValue
                    ? result.LockedUntil.Value.ToString("yyyy-MM-dd HH:mm:ss") + " UTC"
                    : "unknown";

                content =
                    $"Your report on {channel} message #{messageId} has been confirmed as a violation. " +
                    $"The offending player has been chat-locked at level {result.LockLevel} until {lockedUntil}.";
            }
            else if (result.IsToxic)
            {
                content =
                    $"Your report on {channel} message #{messageId} was confirmed as a violation and had already been actioned. " +
                    "No additional automatic chat lock was applied.";
            }
            else
            {
                content =
                    $"Your report on {channel} message #{messageId} has been reviewed. " +
                    "The system did not detect a sufficient violation to apply an automatic chat lock. Thank you for your report.";
            }

            var mailbox = new Mailbox
            {
                PlayerProfileId = reporterId,
                Title = "Report Review Result",
                Content = content,
                Type = "System",
                IsRead = false,
                IsClaimed = false,
                SentAt = DateTime.UtcNow
            };

            await _mailboxRepository.CreateMailbox(mailbox);
        }

        // Executes core business logic for build existing penalty result.
        private static ChatModerationResultDto BuildExistingPenaltyResult(ChatModerationPenalty penalty)
        {
            var now = DateTime.UtcNow;
            var remaining = penalty.LockedUntil > now
                ? (int)Math.Ceiling((penalty.LockedUntil - now).TotalSeconds)
                : 0;

            return new ChatModerationResultDto
            {
                IsToxic = true,
                ChatLocked = remaining > 0,
                LockLevel = penalty.LockLevel,
                ViolationCount = penalty.ViolationCount,
                LockedUntil = penalty.LockedUntil,
                LockDurationSeconds = remaining,
                MatchedTerms = SplitTerms(penalty.MatchedTerms),
                WarningMessage = remaining > 0
                    ? BuildWarningMessage(penalty.LockLevel, penalty.LockedUntil)
                    : "This message has already been reviewed by moderation."
            };
        }

        // Executes core business logic for get lock duration.
        private static TimeSpan GetLockDuration(int lockLevel)
        {
            return lockLevel switch
            {
                1 => FirstLockDuration,
                2 => SecondLockDuration,
                _ => ThirdLockDuration
            };
        }

        // Executes core business logic for build warning message.
        private static string BuildWarningMessage(int lockLevel, DateTime lockedUntil)
        {
            var duration = FormatDuration(GetLockDuration(lockLevel));
            return $"Chat locked for {duration}. Locked until {lockedUntil:yyyy-MM-dd HH:mm:ss} UTC.";
        }

        // Executes core business logic for format duration.
        private static string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalDays >= 3)  return "3 days";
            if (duration.TotalDays >= 1)  return "24 hours";
            return "2 hours";
        }

        // Executes core business logic for trim.
        // Logic details: validates required non-empty string arguments.
        private static string Trim(string? value, int maxLength)
        {
            var normalized = value?.Trim() ?? string.Empty;
            return normalized.Length <= maxLength ? normalized : normalized.Substring(0, maxLength);
        }

        // Executes core business logic for split terms.
        // Logic details: validates required non-empty string arguments.
        private static List<string> SplitTerms(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? new List<string>()
                : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        }
    }
}
