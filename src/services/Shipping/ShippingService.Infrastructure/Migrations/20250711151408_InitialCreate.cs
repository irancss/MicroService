using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shipping");

            migrationBuilder.CreateTable(
                name: "shipping_methods",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    base_cost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    requires_time_slot = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cost_rules = table.Column<string>(type: "jsonb", nullable: true),
                    restriction_rules = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipping_methods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "time_slot_templates",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shipping_method_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_slot_templates", x => x.id);
                    table.ForeignKey(
                        name: "FK_time_slot_templates_shipping_methods_shipping_method_id",
                        column: x => x.shipping_method_id,
                        principalSchema: "shipping",
                        principalTable: "shipping_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "time_slot_bookings",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    time_slot_template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    customer_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    order_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_slot_bookings", x => x.id);
                    table.ForeignKey(
                        name: "FK_time_slot_bookings_time_slot_templates_time_slot_template_id",
                        column: x => x.time_slot_template_id,
                        principalSchema: "shipping",
                        principalTable: "time_slot_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_shipping_methods_is_active",
                schema: "shipping",
                table: "shipping_methods",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_shipping_methods_name",
                schema: "shipping",
                table: "shipping_methods",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_time_slot_bookings_customer_id",
                schema: "shipping",
                table: "time_slot_bookings",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_time_slot_bookings_is_active",
                schema: "shipping",
                table: "time_slot_bookings",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_time_slot_bookings_order_id",
                schema: "shipping",
                table: "time_slot_bookings",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_time_slot_bookings_time_slot_template_id_date",
                schema: "shipping",
                table: "time_slot_bookings",
                columns: new[] { "time_slot_template_id", "date" });

            migrationBuilder.CreateIndex(
                name: "IX_time_slot_templates_is_active",
                schema: "shipping",
                table: "time_slot_templates",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_time_slot_templates_shipping_method_id_day_of_week",
                schema: "shipping",
                table: "time_slot_templates",
                columns: new[] { "shipping_method_id", "day_of_week" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "time_slot_bookings",
                schema: "shipping");

            migrationBuilder.DropTable(
                name: "time_slot_templates",
                schema: "shipping");

            migrationBuilder.DropTable(
                name: "shipping_methods",
                schema: "shipping");
        }
    }
}
