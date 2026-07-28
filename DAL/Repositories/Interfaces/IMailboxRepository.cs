using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý thư tín trong game.
    // Game APIs: Xem thư, nhận thư, đánh dấu đã đọc.
    // Admin APIs: Gửi thư cho người chơi.
    public interface IMailboxRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy thư theo mã định danh, kèm người nhận và vật phẩm đính kèm.
        Task<Mailbox?> GetMailboxById(int id);

        // Lấy tất cả thư của một người chơi, sắp xếp theo thời gian gửi giảm dần.
        Task<List<Mailbox>> GetMailboxesByPlayerId(int playerProfileId);

        // Lấy các thư chưa đọc của người chơi.
        Task<List<Mailbox>> GetUnreadMailboxesByPlayerId(int playerProfileId);

        // Lấy thư của người chơi có phân trang.
        Task<(int TotalCount, List<Mailbox> Items)> GetMailboxesByPlayerIdPaged(int playerProfileId, int page, int pageSize);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo thư mới và gửi đến người chơi.
        Task<Mailbox> CreateMailbox(Mailbox mailbox);

        // Tạo nhiều thư cùng lúc (gửi hàng loạt).
        Task<List<Mailbox>> CreateBulkMailboxes(List<Mailbox> mailboxes);

        // Cập nhật thông tin thư (đánh dấu đã đọc, đã nhận...).
        Task<Mailbox> UpdateMailbox(Mailbox mailbox);

        // Xóa mềm thư (đánh dấu đã xóa).
        Task<Mailbox> SoftDeleteMailbox(int mailboxId);

        // Lấy danh sách thư có phân trang, lọc theo tìm kiếm, trạng thái đọc và nhận.
        Task<(int TotalCount, List<Mailbox> Items)> GetMailboxesPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed, string? sortBy = null, string? sortOrder = null);
    }
}
