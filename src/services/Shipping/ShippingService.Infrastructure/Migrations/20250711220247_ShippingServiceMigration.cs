using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ShippingServiceMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxWeight",
                schema: "shipping",
                table: "shipping_methods",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "free_shipping_rules",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    discount_type = table.Column<int>(type: "integer", nullable: false),
                    discount_value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    max_discount_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    max_usage_count = table.Column<int>(type: "integer", nullable: true),
                    current_usage_count = table.Column<int>(type: "integer", nullable: false),
                    is_premium_only = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_free_shipping_rules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "premium_subscriptions",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    remaining_free_requests = table.Column<int>(type: "integer", nullable: false),
                    max_free_requests_per_month = table.Column<int>(type: "integer", nullable: false),
                    last_reset_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    duration_in_days = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_premium_subscriptions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shipments",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    customer_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    shipping_method_id = table.Column<Guid>(type: "uuid", nullable: false),
                    origin_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    destination_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    origin_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    destination_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    origin_latitude = table.Column<double>(type: "double precision", nullable: false),
                    origin_longitude = table.Column<double>(type: "double precision", nullable: false),
                    destination_latitude = table.Column<double>(type: "double precision", nullable: false),
                    destination_longitude = table.Column<double>(type: "double precision", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    width = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    height = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    length = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    declared_value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    total_cost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    estimated_delivery_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actual_delivery_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tracking_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    optimized_route = table.Column<string>(type: "text", nullable: true),
                    estimated_distance = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    estimated_duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    delivery_driver_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    delivery_driver_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    delivery_driver_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    delivery_notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipments", x => x.id);
                    table.ForeignKey(
                        name: "FK_shipments_shipping_methods_shipping_method_id",
                        column: x => x.shipping_method_id,
                        principalSchema: "shipping",
                        principalTable: "shipping_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "free_shipping_conditions",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    condition_type = table.Column<int>(type: "integer", nullable: false),
                    field_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    @operator = table.Column<int>(name: "operator", type: "integer", nullable: false),
                    value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    value_type = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_free_shipping_conditions", x => x.id);
                    table.ForeignKey(
                        name: "FK_free_shipping_conditions_free_shipping_rules_rule_id",
                        column: x => x.rule_id,
                        principalSchema: "shipping",
                        principalTable: "free_shipping_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription_usage_logs",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shipment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    saved_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    usage_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_usage_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_subscription_usage_logs_premium_subscriptions_subscription_~",
                        column: x => x.subscription_id,
                        principalSchema: "shipping",
                        principalTable: "premium_subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shipment_returns",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    original_shipment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    reason = table.Column<int>(type: "integer", nullable: false),
                    reason_description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    return_tracking_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    requested_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    approved_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    approved_by_user_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    refund_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_refund_processed = table.Column<bool>(type: "boolean", nullable: false),
                    refund_processed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    refund_transaction_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    collection_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    collection_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    collection_notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipment_returns", x => x.id);
                    table.ForeignKey(
                        name: "FK_shipment_returns_shipments_original_shipment_id",
                        column: x => x.original_shipment_id,
                        principalSchema: "shipping",
                        principalTable: "shipments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shipment_trackings",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shipment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    location = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    updated_by_user_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipment_trackings", x => x.id);
                    table.ForeignKey(
                        name: "FK_shipment_trackings_shipments_shipment_id",
                        column: x => x.shipment_id,
                        principalSchema: "shipping",
                        principalTable: "shipments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "return_trackings",
                schema: "shipping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    return_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    updated_by_user_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_return_trackings", x => x.id);
                    table.ForeignKey(
                        name: "FK_return_trackings_shipment_returns_return_id",
                        column: x => x.return_id,
                        principalSchema: "shipping",
                        principalTable: "shipment_returns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_free_shipping_conditions_condition_type",
                schema: "shipping",
                table: "free_shipping_conditions",
                column: "condition_type");

            migrationBuilder.CreateIndex(
                name: "ix_free_shipping_conditions_field_name",
                schema: "shipping",
                table: "free_shipping_conditions",
                column: "field_name");

            migrationBuilder.CreateIndex(
                name: "ix_free_shipping_conditions_rule_id",
                schema: "shipping",
                table: "free_shipping_conditions",
                column: "rule_id");

            migrationBuilder.CreateIndex(
                name: "ix_free_shipping_rules_active_priority",
                schema: "shipping",
                table: "free_shipping_rules",
                columns: new[] { "is_active", "priority" });

            migrationBuilder.CreateIndex(
                name: "ix_free_shipping_rules_end_date",
                schema: "shipping",
                table: "free_shipping_rules",
                column: "end_date");

            migrationBuilder.CreateIndex(
                name: "ix_free_shipping_rules_is_active",
                schema: "shipping",
                table: "free_shipping_rules",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_free_shipping_rules_priority",
                schema: "shipping",
                table: "free_shipping_rules",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "ix_free_shipping_rules_start_date",
                schema: "shipping",
                table: "free_shipping_rules",
                column: "start_date");

            migrationBuilder.CreateIndex(
                name: "ix_premium_subscriptions_end_date",
                schema: "shipping",
                table: "premium_subscriptions",
                column: "end_date");

            migrationBuilder.CreateIndex(
                name: "ix_premium_subscriptions_user_active",
                schema: "shipping",
                table: "premium_subscriptions",
                columns: new[] { "user_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_premium_subscriptions_user_id",
                schema: "shipping",
                table: "premium_subscriptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_return_trackings_return_id",
                schema: "shipping",
                table: "return_trackings",
                column: "return_id");

            migrationBuilder.CreateIndex(
                name: "IX_return_trackings_return_id_timestamp",
                schema: "shipping",
                table: "return_trackings",
                columns: new[] { "return_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_shipment_returns_customer_id",
                schema: "shipping",
                table: "shipment_returns",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipment_returns_original_shipment_id",
                schema: "shipping",
                table: "shipment_returns",
                column: "original_shipment_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipment_returns_return_tracking_number",
                schema: "shipping",
                table: "shipment_returns",
                column: "return_tracking_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shipment_returns_status",
                schema: "shipping",
                table: "shipment_returns",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_shipment_returns_status_requested_date",
                schema: "shipping",
                table: "shipment_returns",
                columns: new[] { "status", "requested_date" });

            migrationBuilder.CreateIndex(
                name: "IX_shipment_trackings_shipment_id",
                schema: "shipping",
                table: "shipment_trackings",
                column: "shipment_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipment_trackings_shipment_id_timestamp",
                schema: "shipping",
                table: "shipment_trackings",
                columns: new[] { "shipment_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_shipments_customer_id",
                schema: "shipping",
                table: "shipments",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipments_delivery_driver_id",
                schema: "shipping",
                table: "shipments",
                column: "delivery_driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipments_order_id",
                schema: "shipping",
                table: "shipments",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipments_shipping_method_id",
                schema: "shipping",
                table: "shipments",
                column: "shipping_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipments_status",
                schema: "shipping",
                table: "shipments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_shipments_status_estimated_delivery_date",
                schema: "shipping",
                table: "shipments",
                columns: new[] { "status", "estimated_delivery_date" });

            migrationBuilder.CreateIndex(
                name: "IX_shipments_tracking_number",
                schema: "shipping",
                table: "shipments",
                column: "tracking_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscription_usage_logs_shipment_id",
                schema: "shipping",
                table: "subscription_usage_logs",
                column: "shipment_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_usage_logs_subscription_id",
                schema: "shipping",
                table: "subscription_usage_logs",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_usage_logs_usage_date",
                schema: "shipping",
                table: "subscription_usage_logs",
                column: "usage_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "free_shipping_conditions",
                schema: "shipping");

            migrationBuilder.DropTable(
                name: "return_trackings",
                schema: "shipping");

            migrationBuilder.DropTable(
                name: "shipment_trackings",
                schema: "shipping");

            migrationBuilder.DropTable(
                name: "subscription_usage_logs",
                schema: "shipping");

            migrationBuilder.DropTable(
                name: "free_shipping_rules",
                schema: "shipping");

            migrationBuilder.DropTable(
                name: "shipment_returns",
                schema: "shipping");

            migrationBuilder.DropTable(
                name: "premium_subscriptions",
                schema: "shipping");

            migrationBuilder.DropTable(
                name: "shipments",
                schema: "shipping");

            migrationBuilder.DropColumn(
                name: "MaxWeight",
                schema: "shipping",
                table: "shipping_methods");
        }
    }
}
