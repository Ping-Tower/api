using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.MigrationManager
{
    /// <inheritdoc />
    public partial class MoveRequestQueryToServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Query",
                table: "Servers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE "Servers" AS s
                SET "Query" = r."Query"
                FROM (
                    SELECT DISTINCT ON ("ServerId") "ServerId", "Query"
                    FROM "Requests"
                    WHERE NOT "IsDeleted"
                      AND "ServerId" IS NOT NULL
                    ORDER BY "ServerId", "CreatedAt", "Id"
                ) AS r
                WHERE s."Id" = r."ServerId"
                  AND s."Query" IS NULL;
                """);

            migrationBuilder.DropTable(
                name: "Requests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ServerId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Query = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ServerId",
                table: "Requests",
                column: "ServerId");

            migrationBuilder.Sql(
                """
                INSERT INTO "Requests" (
                    "Id",
                    "ServerId",
                    "CreatedAt",
                    "CreatedBy",
                    "IsDeleted",
                    "ModifiedAt",
                    "ModifiedBy",
                    "Query"
                )
                SELECT
                    "Id" || '-request',
                    "Id",
                    "CreatedAt",
                    "CreatedBy",
                    "IsDeleted",
                    "ModifiedAt",
                    "ModifiedBy",
                    "Query"
                FROM "Servers"
                WHERE "Query" IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "Query",
                table: "Servers");
        }
    }
}
