using System;

namespace BLL.Utils
{
    /// <summary>
    /// Ngưỡng thời gian để coi một tài khoản là "đang online", tính từ Account.LastSeen.
    ///
    /// Chỉ có DUY NHẤT game client ghi LastSeen (heartbeat mỗi <see cref="HeartbeatSeconds"/> giây,
    /// xem HeartbeatSender.cs bên Unity). Web portal đăng nhập với clientType "Web" nên không ghi cột này.
    ///
    /// Trước đây mỗi tính năng tự hardcode ngưỡng riêng (5 phút ở FriendService/GuildService,
    /// 1 phút ở DashboardService) và không chỗ nào nhắc tới heartbeat interval. Đổi interval bên
    /// Unity thì mọi ngưỡng đều lệch theo mà không ai thấy. Gom về đây để quan hệ giữa
    /// interval và ngưỡng là hiển nhiên.
    /// </summary>
    public static class OnlineTimeout
    {
        /// <summary>Chu kỳ heartbeat của game client, phải khớp HeartbeatSender.heartbeatInterval (Unity).</summary>
        public const int HeartbeatSeconds = 30;

        /// <summary>
        /// Ngưỡng "vừa hoạt động" cho presence trong danh sách bạn bè và bang hội.
        /// Rộng có chủ đích: mất mạng chốc lát không nên làm bạn bè thấy mình offline.
        /// </summary>
        public static readonly TimeSpan Presence = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Ngưỡng đếm "đang online ngay lúc này" cho dashboard admin. Hẹp có chủ đích để số liệu
        /// phản ánh hiện tại. Lưu ý chỉ bằng 2× heartbeat, nên client trượt một nhịp là bị đếm
        /// offline — muốn số liệu bớt nhiễu thì nới hằng số này, đừng sửa từng chỗ gọi.
        /// </summary>
        public static readonly TimeSpan Dashboard = TimeSpan.FromMinutes(1);

        /// <summary>
        /// True nếu <paramref name="lastSeen"/> còn nằm trong <paramref name="window"/> so với hiện tại.
        /// LastSeen null = chưa từng online từ game client (hoặc đã logout, vì logout xoá cột này).
        /// </summary>
        public static bool IsWithin(DateTime? lastSeen, TimeSpan window)
            => lastSeen.HasValue && lastSeen.Value >= DateTime.UtcNow - window;
    }
}
