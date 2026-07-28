using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class WorldChatFeedIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorldChatMessages_SentAt",
                table: "WorldChatMessages");

            // Ghi chú: scaffolder còn sinh thêm DropColumn "DungeonConfigs.ImageUrl" — đó là
            // drift có từ trước (model đã bỏ cột nhưng chưa ai tạo migration), không liên quan
            // tới index này và là thao tác mất dữ liệu. Đã bỏ ra khỏi migration; cần xử lý riêng.
            migrationBuilder.CreateIndex(
                name: "IX_WorldChatMessages_Feed",
                table: "WorldChatMessages",
                columns: new[] { "IsHidden", "SentAt", "WorldChatMessageId" },
                descending: new[] { false, true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorldChatMessages_Feed",
                table: "WorldChatMessages");

            migrationBuilder.CreateIndex(
                name: "IX_WorldChatMessages_SentAt",
                table: "WorldChatMessages",
                column: "SentAt");
        }
    }
}
