using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý thư (mail) cho người chơi và admin.
    // Game APIs: Người chơi xem, đọc, nhận thưởng, xóa mail của mình.
    // Admin APIs: Admin gửi mail, broadcast, và quản lý tất cả mail.
    public interface IMailService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách mail của player đang đăng nhập, có phân trang.
        Task<MailListPagedDto> GetMyMails(int playerProfileId, int page, int pageSize);

        // Lấy chi tiết mail theo MailId.
        Task<MailDetailDto?> GetMailById(int mailId);

        // Đánh dấu mail đã đọc.
        Task<MailDetailDto> MarkMailAsRead(int mailId);

        // Nhận phần thưởng trong mail (gold, gems, item).
        Task<MailDetailDto> ClaimMailReward(int mailId);

        // Xóa mail của player đang đăng nhập.
        Task DeleteMail(int mailId, int playerProfileId);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy tất cả mail có lọc và phân trang (Admin).
        Task<PagedResultDto<MailDetailDto>> GetMailsPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed);

        // Gửi mail đến danh sách player theo ID.
        Task SendMailByListId(SendMailByListIdDto request);

        // Broadcast mail đến tất cả player.
        Task SendMailToAll(SendMailToAllDto request);
    }
}
