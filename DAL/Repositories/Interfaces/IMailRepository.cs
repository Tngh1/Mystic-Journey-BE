using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý thư tín trong game.
    // Game APIs: Xem thư, nhận thư, đánh dấu đã đọc.
    // Admin APIs: Gửi thư cho người chơi.
    public interface IMailRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy thư theo mã định danh, kèm người nhận và vật phẩm đính kèm.
        Task<Mail?> GetMailById(int id);

        // Lấy tất cả thư của một người chơi, sắp xếp theo thời gian gửi giảm dần.
        Task<List<Mail>> GetMailsByPlayerId(int playerProfileId);

        // Lấy các thư chưa đọc của người chơi.
        Task<List<Mail>> GetUnreadMailsByPlayerId(int playerProfileId);

        // Lấy thư của người chơi có phân trang.
        Task<(int TotalCount, List<Mail> Items)> GetMailsByPlayerIdPaged(int playerProfileId, int page, int pageSize);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo thư mới và gửi đến người chơi.
        Task<Mail> CreateMail(Mail mail);

        // Tạo nhiều thư cùng lúc (gửi hàng loạt).
        Task<List<Mail>> CreateBulkMails(List<Mail> mails);

        // Cập nhật thông tin thư (đánh dấu đã đọc, đã nhận...).
        Task<Mail> UpdateMail(Mail mail);

        // Xóa mềm thư (đánh dấu đã xóa).
        Task<Mail> SoftDeleteMail(int mailId);

        // Lấy danh sách thư có phân trang, lọc theo tìm kiếm, trạng thái đọc và nhận.
        Task<(int TotalCount, List<Mail> Items)> GetMailsPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed);
    }
}
