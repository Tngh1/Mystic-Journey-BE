using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddChatModerationPenalties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReportReason",
                table: "ChatMessages",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReportedAt",
                table: "ChatMessages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportedById",
                table: "ChatMessages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChatModerationPenalties",
                columns: table => new
                {
                    ChatModerationPenaltyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    ReporterId = table.Column<int>(type: "integer", nullable: true),
                    ChatMessageId = table.Column<int>(type: "integer", nullable: true),
                    WorldChatMessageId = table.Column<int>(type: "integer", nullable: true),
                    Channel = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ContentSnapshot = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ReportReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MatchedTerms = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ViolationCount = table.Column<int>(type: "integer", nullable: false),
                    LockLevel = table.Column<int>(type: "integer", nullable: false),
                    LockedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatModerationPenalties", x => x.ChatModerationPenaltyId);
                    table.ForeignKey(
                        name: "FK_ChatModerationPenalties_ChatMessages_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "ChatMessageId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChatModerationPenalties_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatModerationPenalties_PlayerProfiles_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChatModerationPenalties_WorldChatMessages_WorldChatMessageId",
                        column: x => x.WorldChatMessageId,
                        principalTable: "WorldChatMessages",
                        principalColumn: "WorldChatMessageId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ReportedById",
                table: "ChatMessages",
                column: "ReportedById");

            migrationBuilder.CreateIndex(
                name: "IX_ChatModerationPenalties_ChatMessageId",
                table: "ChatModerationPenalties",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatModerationPenalties_LockedUntil",
                table: "ChatModerationPenalties",
                column: "LockedUntil");

            migrationBuilder.CreateIndex(
                name: "IX_ChatModerationPenalties_PlayerProfileId",
                table: "ChatModerationPenalties",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatModerationPenalties_ReporterId",
                table: "ChatModerationPenalties",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatModerationPenalties_WorldChatMessageId",
                table: "ChatModerationPenalties",
                column: "WorldChatMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_PlayerProfiles_ReportedById",
                table: "ChatMessages",
                column: "ReportedById",
                principalTable: "PlayerProfiles",
                principalColumn: "PlayerProfileId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_PlayerProfiles_ReportedById",
                table: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatModerationPenalties");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ReportedById",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ReportReason",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ReportedAt",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ReportedById",
                table: "ChatMessages");
        }
    }
}
