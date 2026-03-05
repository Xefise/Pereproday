using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pereprodai.Moderation.Migrations
{
    /// <inheritdoc />
    public partial class AddModerationTaskAndAdReadModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "moderation");

            migrationBuilder.CreateTable(
                name: "ads_read_model",
                schema: "moderation",
                columns: table => new
                {
                    AdId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    PriceAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceCurrency = table.Column<int>(type: "integer", nullable: false),
                    LocationCity = table.Column<string>(type: "text", nullable: false),
                    ContactInfoPhone = table.Column<string>(type: "text", nullable: false),
                    ContactInfoEmail = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ads_read_model", x => x.AdId);
                });

            migrationBuilder.CreateTable(
                name: "moderation_tasks",
                schema: "moderation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RejectionReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ModeratorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AdId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moderation_tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_moderation_tasks_ads_read_model_AdId",
                        column: x => x.AdId,
                        principalSchema: "moderation",
                        principalTable: "ads_read_model",
                        principalColumn: "AdId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_moderation_tasks_AdId",
                schema: "moderation",
                table: "moderation_tasks",
                column: "AdId");

            migrationBuilder.CreateIndex(
                name: "IX_moderation_tasks_Status_CreatedAt",
                schema: "moderation",
                table: "moderation_tasks",
                columns: new[] { "Status", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "moderation_tasks",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "ads_read_model",
                schema: "moderation");
        }
    }
}
