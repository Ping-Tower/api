using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTelegramWidgetLinking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AuthDateUtc",
                table: "TelegramAccount",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "TelegramAccount",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "TelegramAccount",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TelegramUserId",
                table: "TelegramAccount",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "TelegramAccount",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelegramAccount_TelegramUserId",
                table: "TelegramAccount",
                column: "TelegramUserId",
                unique: true);

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "TelegramAccount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TelegramAccount_TelegramUserId",
                table: "TelegramAccount");

            migrationBuilder.DropColumn(
                name: "AuthDateUtc",
                table: "TelegramAccount");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "TelegramAccount");

            migrationBuilder.DropColumn(
                name: "TelegramUserId",
                table: "TelegramAccount");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "TelegramAccount");

            migrationBuilder.AddColumn<string>(
                name: "ChatId",
                table: "TelegramAccount",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "TelegramAccount");
        }
    }
}
