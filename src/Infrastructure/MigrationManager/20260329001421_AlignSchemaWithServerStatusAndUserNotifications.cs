using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.MigrationManager
{
    /// <inheritdoc />
    public partial class AlignSchemaWithServerStatusAndUserNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationSettings_Servers_ServerId",
                table: "NotificationSettings");

            migrationBuilder.DropTable(
                name: "ServerState");

            migrationBuilder.DropColumn(
                name: "LatencyTresholdMs",
                table: "NotificationSettings");

            migrationBuilder.RenameColumn(
                name: "FailtureThreshold",
                table: "PingSettings",
                newName: "FailureThreshold");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "NotificationSettings",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationSettings_ServerId",
                table: "NotificationSettings",
                newName: "IX_NotificationSettings_UserId");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Servers",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "LatencyThresholdMs",
                table: "PingSettings",
                type: "integer",
                nullable: false,
                defaultValue: 400);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationSettings_AspNetUsers_UserId",
                table: "NotificationSettings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationSettings_AspNetUsers_UserId",
                table: "NotificationSettings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "LatencyThresholdMs",
                table: "PingSettings");

            migrationBuilder.RenameColumn(
                name: "FailureThreshold",
                table: "PingSettings",
                newName: "FailtureThreshold");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "NotificationSettings",
                newName: "ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationSettings_UserId",
                table: "NotificationSettings",
                newName: "IX_NotificationSettings_ServerId");

            migrationBuilder.AddColumn<int>(
                name: "LatencyTresholdMs",
                table: "NotificationSettings",
                type: "integer",
                nullable: false,
                defaultValue: 400);

            migrationBuilder.CreateTable(
                name: "ServerState",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ServerId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    FailCount = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsUp = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_ServerState_ServerId",
                table: "ServerState",
                column: "ServerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationSettings_Servers_ServerId",
                table: "NotificationSettings",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id");
        }
    }
}
