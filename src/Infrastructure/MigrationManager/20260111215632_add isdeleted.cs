using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.MigrationManager
{
    /// <inheritdoc />
    public partial class AddIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TelegramAccount",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ServerState",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Servers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Requests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PingSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "NotificationSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TelegramAccount");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ServerState");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PingSettings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "NotificationSettings");
        }
    }
}
