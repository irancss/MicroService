using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscountService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CouponCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsAutomatic = table.Column<bool>(type: "boolean", nullable: false),
                    IsCombinableWithOthers = table.Column<bool>(type: "boolean", nullable: false),
                    MaxTotalUsage = table.Column<int>(type: "integer", nullable: true),
                    MaxUsagePerUser = table.Column<int>(type: "integer", nullable: true),
                    CurrentTotalUsage = table.Column<int>(type: "integer", nullable: false),
                    MinimumCartAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    MaximumDiscountAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Applicability = table.Column<int>(type: "integer", nullable: false),
                    ApplicableProductIds = table.Column<string>(type: "text", nullable: true),
                    ApplicableCategoryIds = table.Column<string>(type: "text", nullable: true),
                    BuyQuantity = table.Column<int>(type: "integer", nullable: true),
                    GetQuantity = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountUsageHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscountId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CartTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CouponCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountUsageHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountUsageHistories_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CouponCode",
                table: "Discounts",
                column: "CouponCode",
                unique: true,
                filter: "\"CouponCode\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_EndDate",
                table: "Discounts",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_IsActive",
                table: "Discounts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_IsAutomatic",
                table: "Discounts",
                column: "IsAutomatic");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_StartDate",
                table: "Discounts",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_UserId",
                table: "Discounts",
                column: "UserId",
                filter: "\"UserId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountUsageHistories_DiscountId",
                table: "DiscountUsageHistories",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountUsageHistories_OrderId",
                table: "DiscountUsageHistories",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountUsageHistories_UsedAt",
                table: "DiscountUsageHistories",
                column: "UsedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountUsageHistories_UserId",
                table: "DiscountUsageHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountUsageHistories");

            migrationBuilder.DropTable(
                name: "Discounts");
        }
    }
}
