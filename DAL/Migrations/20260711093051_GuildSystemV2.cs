using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class GuildSystemV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_PlayerProfiles_LeaderId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "GuildMembers");

            migrationBuilder.RenameColumn(
                name: "MaxMembers",
                table: "Guilds",
                newName: "JoinPolicy");

            migrationBuilder.RenameColumn(
                name: "Experience",
                table: "Guilds",
                newName: "GuildExp");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLeaveAt",
                table: "PlayerProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "Guilds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByProfileId",
                table: "Guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                ALTER TABLE ""GuildMembers"" 
                ALTER COLUMN ""Role"" TYPE integer 
                USING CASE ""Role""
                    WHEN 'Leader' THEN 2
                    WHEN 'Officer' THEN 1
                    ELSE 0
                END;
            ");

            migrationBuilder.AddColumn<int>(
                name: "DailyContribution",
                table: "GuildMembers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDonateAt",
                table: "GuildMembers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalContribution",
                table: "GuildMembers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeeklyContribution",
                table: "GuildMembers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MessageType",
                table: "GuildChatMessages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GuildLogs",
                columns: table => new
                {
                    GuildLogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<int>(type: "integer", nullable: false),
                    ActorProfileId = table.Column<int>(type: "integer", nullable: true),
                    TargetProfileId = table.Column<int>(type: "integer", nullable: true),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    Detail = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildLogs", x => x.GuildLogId);
                    table.ForeignKey(
                        name: "FK_GuildLogs_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildLogs_PlayerProfiles_ActorProfileId",
                        column: x => x.ActorProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GuildLogs_PlayerProfiles_TargetProfileId",
                        column: x => x.TargetProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_CreatedByProfileId",
                table: "Guilds",
                column: "CreatedByProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildLogs_ActorProfileId",
                table: "GuildLogs",
                column: "ActorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildLogs_GuildId",
                table: "GuildLogs",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildLogs_TargetProfileId",
                table: "GuildLogs",
                column: "TargetProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_PlayerProfiles_CreatedByProfileId",
                table: "Guilds",
                column: "CreatedByProfileId",
                principalTable: "PlayerProfiles",
                principalColumn: "PlayerProfileId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_PlayerProfiles_LeaderId",
                table: "Guilds",
                column: "LeaderId",
                principalTable: "PlayerProfiles",
                principalColumn: "PlayerProfileId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_PlayerProfiles_CreatedByProfileId",
                table: "Guilds");

            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_PlayerProfiles_LeaderId",
                table: "Guilds");

            migrationBuilder.DropTable(
                name: "GuildLogs");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_CreatedByProfileId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "LastLeaveAt",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "CreatedByProfileId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "DailyContribution",
                table: "GuildMembers");

            migrationBuilder.DropColumn(
                name: "LastDonateAt",
                table: "GuildMembers");

            migrationBuilder.DropColumn(
                name: "TotalContribution",
                table: "GuildMembers");

            migrationBuilder.DropColumn(
                name: "WeeklyContribution",
                table: "GuildMembers");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "GuildChatMessages");

            migrationBuilder.RenameColumn(
                name: "JoinPolicy",
                table: "Guilds",
                newName: "MaxMembers");

            migrationBuilder.RenameColumn(
                name: "GuildExp",
                table: "Guilds",
                newName: "Experience");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "GuildMembers",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "GuildMembers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_PlayerProfiles_LeaderId",
                table: "Guilds",
                column: "LeaderId",
                principalTable: "PlayerProfiles",
                principalColumn: "PlayerProfileId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
