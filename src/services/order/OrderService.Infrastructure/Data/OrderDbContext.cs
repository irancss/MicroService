using BuildingBlocks.Infrastructure.Data;
using BuildingBlocks.Messaging.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using OrderService.Core.Models;

namespace OrderService.Infrastructure.Data
{
    /// <summary>
    /// This class is the main database context for managing orders.
    /// </summary>
    public class OrderDbContext : DbContext, IApplicationDbContext
    {
        /// <summary>
        /// Constructor that retrieves the required configurations using DbContextOptions.
        /// </summary>
        /// <param name="options">Initial settings for connecting to the database</param>
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// The main orders table.
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// The order items table.
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; }

        // پیاده‌سازی نیازمندی‌های اینترفیس
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        // [اصلاح شد] پیاده‌سازی پراپرتی جدید از اینترفیس
        public DbSet<StoredEvent> StoredEvents { get; set; }



        /// <summary>
        /// Configure the model (tables and columns) using Fluent API.
        /// </summary>
        /// <param name="modelBuilder">A tool for building and configuring the database model</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Default configuration for PostgreSQL
            modelBuilder.HasDefaultSchema("public");

            // Configure the orders table
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).HasColumnName("id");
                entity.Property(o => o.CustomerId).HasColumnName("customer_id");
                entity.Property(o => o.TotalPrice).HasColumnName("total_price");
                entity.Property(o => o.TotalDiscount).HasColumnName("total_discount");
                entity.Property(o => o.Status).HasColumnName("status");
                entity.Property(o => o.ShippingAddress).HasColumnName("shipping_address");
                entity.Property(o => o.BillingAddress).HasColumnName("billing_address");
                entity.Property(o => o.PaymentStatus).HasColumnName("payment_status");
                entity.Property(o => o.TotalAmount).HasColumnName("total_amount");
                entity.Property(o => o.CreatedAt).HasColumnName("created_at");
                entity.Property(o => o.LastUpdatedAt).HasColumnName("last_updated_at");
                entity.Property(o => o.LastUpdatedBy).HasColumnName("last_updated_by");
                entity.HasMany(o => o.Items)
                      .WithOne()
                      .HasForeignKey(oi => oi.OrderId);
            });

            // Configure the order_items table
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_items");
                entity.HasKey(oi => oi.Id);
                entity.Property(oi => oi.Id).HasColumnName("id");
                entity.Property(oi => oi.OrderId).HasColumnName("order_id");
                entity.Property(oi => oi.ProductId).HasColumnName("product_id");
                entity.Property(oi => oi.ProductName).HasColumnName("product_name");
                entity.Property(oi => oi.Quantity).HasColumnName("quantity");
                entity.Property(oi => oi.UnitPrice).HasColumnName("unit_price");
                entity.Property(oi => oi.Discount).HasColumnName("discount");
                entity.Property(oi => oi.TotalPrice).HasColumnName("total_price");
            });
        }
    }

    /// <summary>
    /// Initial migration class for creating required database tables.
    /// </summary>
    public partial class InitialCreate : Migration
    {
        /// <summary>
        /// Operations to create tables and insert initial data into the database.
        /// </summary>
        /// <param name="migrationBuilder">Tool for applying migration operations</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    customer_id = table.Column<Guid>(nullable: false),
                    total_price = table.Column<decimal>(type: "numeric", nullable: false),
                    total_discount = table.Column<decimal>(type: "numeric", nullable: false),
                    status = table.Column<int>(nullable: false),
                    shipping_address = table.Column<string>(nullable: true),
                    billing_address = table.Column<string>(nullable: true),
                    payment_status = table.Column<int>(nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    last_updated_at = table.Column<DateTime>(nullable: false),
                    last_updated_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    order_id = table.Column<Guid>(nullable: false),
                    product_id = table.Column<string>(nullable: true),
                    product_name = table.Column<string>(nullable: true),
                    quantity = table.Column<int>(nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: false),
                    discount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_items_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "public",
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_items_order_id",
                schema: "public",
                table: "order_items",
                column: "order_id");

            // Insert some sample orders
            var order1Id = Guid.NewGuid();
            var order2Id = Guid.NewGuid();

            migrationBuilder.InsertData(
                schema: "public",
                table: "orders",
                columns: new[] { "id", "customer_id", "total_price", "total_discount", "status", "shipping_address", "billing_address", "payment_status", "total_amount", "created_at", "last_updated_at", "last_updated_by" },
                values: new object[,]
                {
                    { order1Id, Guid.NewGuid(), 100.0m, 10.0m, 0, "Address 1", "Billing 1", 1, 90.0m, DateTime.UtcNow, DateTime.UtcNow, "seed" },
                    { order2Id, Guid.NewGuid(), 200.0m, 20.0m, 1, "Address 2", "Billing 2", 0, 180.0m, DateTime.UtcNow, DateTime.UtcNow, "seed" }
                }
            );

            // Insert some sample items for the orders
            migrationBuilder.InsertData(
                schema: "public",
                table: "order_items",
                columns: new[] { "id", "order_id", "product_id", "product_name", "quantity", "unit_price", "discount", "total_price" },
                values: new object[,]
                {
                    { Guid.NewGuid(), order1Id, "P1", "Product 1", 2, 50.0m, 5.0m, 95.0m },
                    { Guid.NewGuid(), order2Id, "P2", "Product 2", 1, 200.0m, 20.0m, 180.0m }
                }
            );
        }

        /// <summary>
        /// Operations to drop tables if a rollback is needed.
        /// </summary>
        /// <param name="migrationBuilder">Tool for rolling back migrations</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_items",
                schema: "public");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "public");
        }
    }
}
