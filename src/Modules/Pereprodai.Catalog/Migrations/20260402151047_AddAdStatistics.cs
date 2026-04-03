using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pereprodai.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class AddAdStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "adStatistics",
                schema: "catalog",
                columns: table => new
                {
                    AdId = table.Column<Guid>(type: "uuid", nullable: false),
                    Views = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adStatistics", x => x.AdId);
                    table.ForeignKey(
                        name: "FK_adStatistics_ads_AdId",
                        column: x => x.AdId,
                        principalSchema: "catalog",
                        principalTable: "ads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adStatistics",
                schema: "catalog");
        }
    }
}
