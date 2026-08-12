using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý thư (mailbox) cho người chơi và admin.
    // Game APIs: Người chơi xem, đọc, nhận thưởng, xóa thư của mình.
    // Admin APIs: Admin gửi thư, broadcast, và quản lý tất cả thư.
    public interface IMailboxService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách thư của player đang đăng nhập, có phân trang.
        Task<MailboxListPagedDto> GetMyMailboxes(int playerProfileId, int page, int pageSize);

        // Lấy chi tiết thư theo MailboxId.
        Task<MailboxDetailDto?> GetMailboxById(int mailboxId);

        // Đánh dấu thư đã đọc.
        Task<MailboxDetailDto> MarkMailboxAsRead(int mailboxId);

        // Nhận phần thưởng trong thư (gold, gems, item).
        Task<MailboxDetailDto> ClaimMailboxReward(int mailboxId);

        // Xóa thư của player đang đăng nhập.
        Task DeleteMailbox(int mailboxId, int playerProfileId);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy tất cả thư có lọc và phân trang (Admin).
        Task<PagedResultDto<MailboxDetailDto>> GetMailboxesPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed, string? sortBy = null, string? sortOrder = null);

        // Gửi thư đến danh sách player theo ID.
        Task<List<MailboxDetailDto>> SendMailboxByListId(SendMailboxByListIdDto request);

        // Broadcast thư đến tất cả player.
        Task<List<MailboxDetailDto>> SendMailboxToAll(SendMailboxToAllDto request);
    }
}
