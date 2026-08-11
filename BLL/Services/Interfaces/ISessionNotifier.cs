namespace BLL.Services.Interfaces
{
    // Kênh đẩy thông báo "phiên của bạn vừa bị đè" xuống thiết bị cũ, hiện thực bằng SignalR ở
    // tầng API. Khai báo interface ở BLL để AuthService không phải tham chiếu ASP.NET Core —
    // BLL chỉ biết "có ai đó thông báo được", không biết bằng WebSocket hay gì khác.
    //
    // Đây CHỈ là kênh thông báo nhanh, KHÔNG phải nguồn sự thật của việc kick. Nguồn sự thật vẫn
    // là khoá active_session trong Redis + slot refresh token: client có thể không nối hub, có
    // thể mất mạng, hoặc nối vào instance API khác. Mọi trường hợp đó vẫn bị đá đúng ở request
    // kế tiếp qua OnTokenValidated — SignalR chỉ làm nó xảy ra ngay thay vì phải chờ.
    public interface ISessionNotifier
    {
        // clientType quyết định nhóm nhận: đá phiên game không được đụng tới phiên web cùng tài khoản.
        // newSessionId là phiên vừa chiếm chỗ; truyền chuỗi rỗng khi phiên bị thu hồi mà không có
        // phiên nào kế nhiệm (ví dụ đổi mật khẩu revoke phía client kia).
        Task SessionOverridden(int accountId, string clientType, string newSessionId);
    }
}
