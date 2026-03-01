using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pereprodai.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class AddAd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.CreateTable(
                name: "ads",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Price_Currency = table.Column<int>(type: "integer", nullable: false),
                    Location_City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContactInfo_Phone = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    ContactInfo_Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ads", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ads_Category",
                schema: "catalog",
                table: "ads",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ads_Status",
                schema: "catalog",
                table: "ads",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ads_UserId",
                schema: "catalog",
                table: "ads",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ads",
                schema: "catalog");
        }
    }
}
