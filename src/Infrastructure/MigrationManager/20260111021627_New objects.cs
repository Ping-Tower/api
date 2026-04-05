using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.MigrationManager
{
    /// <inheritdoc />
    public partial class Newobjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckIntervalSec",
                table: "Servers");

            migrationBuilder.RenameColumn(
                name: "ProtocolRef",
                table: "Servers",
                newName: "Protocol");

            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ServerId = table.Column<string>(type: "text", nullable: true),
                    OnDown = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OnUp = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OnLatency = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LatencyTresholdMs = table.Column<int>(type: "integer", nullable: false, defaultValue: 400),
                    CooldownSec = table.Column<int>(type: "integer", nullable: false, defaultValue: 600),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationSettings_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PingSettings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ServerId = table.Column<string>(type: "text", nullable: true),
                    IntervalSec = table.Column<int>(type: "integer", nullable: false, defaultValue: 60),
                    Retries = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FailtureThreshold = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PingSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PingSettings_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServerState",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ServerId = table.Column<string>(type: "text", nullable: true),
                    IsUp = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FailCount = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerState_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TelegramAccount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ChatId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelegramAccount_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettings_ServerId",
                table: "NotificationSettings",
                column: "ServerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PingSettings_ServerId",
                table: "PingSettings",
                column: "ServerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServerState_ServerId",
                table: "ServerState",
                column: "ServerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelegramAccount_UserId",
                table: "TelegramAccount",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationSettings");

            migrationBuilder.DropTable(
                name: "PingSettings");

            migrationBuilder.DropTable(
                name: "ServerState");

            migrationBuilder.DropTable(
                name: "TelegramAccount");

            migrationBuilder.RenameColumn(
                name: "Protocol",
                table: "Servers",
                newName: "ProtocolRef");

            migrationBuilder.AddColumn<int>(
                name: "CheckIntervalSec",
                table: "Servers",
                type: "integer",
                nullable: false,
                defaultValue: 60);
        }
    }
}
