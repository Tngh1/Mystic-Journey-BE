using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class GuildSystemV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "Guilds");

            migrationBuilder.AlterColumn<string>(
                name: "Notice",
                table: "Guilds",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "BannerId",
                table: "Guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IconId",
                table: "Guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastChatAt",
                table: "GuildMembers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActorName",
                table: "GuildLogs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetName",
                table: "GuildLogs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "GuildInvitations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SenderRole",
                table: "GuildChatMessages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "IconId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "LastChatAt",
                table: "GuildMembers");

            migrationBuilder.DropColumn(
                name: "ActorName",
                table: "GuildLogs");

            migrationBuilder.DropColumn(
                name: "TargetName",
                table: "GuildLogs");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "GuildInvitations");

            migrationBuilder.DropColumn(
                name: "SenderRole",
                table: "GuildChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "Notice",
                table: "Guilds",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "Guilds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "Guilds",
                type: "text",
                nullable: true);
        }
    }
}
