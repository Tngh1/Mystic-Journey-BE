using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class GuildSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GuildMembers_PlayerProfileId",
                table: "GuildMembers");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveTime",
                table: "PlayerProfiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notice",
                table: "Guilds",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RequiredLevel",
                table: "Guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalMedals",
                table: "Guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Feats",
                table: "GuildMembers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Medals",
                table: "GuildMembers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GuildApplications",
                columns: table => new
                {
                    GuildApplicationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<int>(type: "integer", nullable: false),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildApplications", x => x.GuildApplicationId);
                    table.ForeignKey(
                        name: "FK_GuildApplications_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildApplications_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildChatMessages",
                columns: table => new
                {
                    GuildChatMessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildChatMessages", x => x.GuildChatMessageId);
                    table.ForeignKey(
                        name: "FK_GuildChatMessages_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildChatMessages_PlayerProfiles_SenderId",
                        column: x => x.SenderId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildMembers_PlayerProfileId",
                table: "GuildMembers",
                column: "PlayerProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuildApplications_GuildId",
                table: "GuildApplications",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildApplications_PlayerProfileId",
                table: "GuildApplications",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildChatMessages_GuildId",
                table: "GuildChatMessages",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildChatMessages_SenderId",
                table: "GuildChatMessages",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildApplications");

            migrationBuilder.DropTable(
                name: "GuildChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_GuildMembers_PlayerProfileId",
                table: "GuildMembers");

            migrationBuilder.DropColumn(
                name: "LastActiveTime",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "Notice",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "RequiredLevel",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "TotalMedals",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "Feats",
                table: "GuildMembers");

            migrationBuilder.DropColumn(
                name: "Medals",
                table: "GuildMembers");

            migrationBuilder.CreateIndex(
                name: "IX_GuildMembers_PlayerProfileId",
                table: "GuildMembers",
                column: "PlayerProfileId");
        }
    }
}
