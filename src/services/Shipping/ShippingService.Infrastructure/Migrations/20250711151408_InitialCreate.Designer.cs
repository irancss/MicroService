﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ShippingService.Infrastructure.Data;

#nullable disable

namespace ShippingService.Infrastructure.Migrations
{
    [DbContext(typeof(ShippingDbContext))]
    [Migration("20250711151408_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("shipping")
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ShippingService.Domain.Entities.ShippingMethod", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal>("BaseCost")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("base_cost");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("created_by");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("description");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("name");

                    b.Property<bool>("RequiresTimeSlot")
                        .HasColumnType("boolean")
                        .HasColumnName("requires_time_slot");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("updated_by");

                    b.HasKey("Id");

                    b.HasIndex("IsActive");

                    b.HasIndex("Name");

                    b.ToTable("shipping_methods", "shipping");
                });

            modelBuilder.Entity("ShippingService.Domain.Entities.TimeSlotBooking", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("customer_id");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date")
                        .HasColumnName("date");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time without time zone")
                        .HasColumnName("end_time");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("OrderId")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("order_id");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time without time zone")
                        .HasColumnName("start_time");

                    b.Property<Guid>("TimeSlotTemplateId")
                        .HasColumnType("uuid")
                        .HasColumnName("time_slot_template_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("IsActive");

                    b.HasIndex("OrderId");

                    b.HasIndex("TimeSlotTemplateId", "Date");

                    b.ToTable("time_slot_bookings", "shipping");
                });

            modelBuilder.Entity("ShippingService.Domain.Entities.TimeSlotTemplate", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("Capacity")
                        .HasColumnType("integer")
                        .HasColumnName("capacity");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("integer")
                        .HasColumnName("day_of_week");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time without time zone")
                        .HasColumnName("end_time");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<Guid>("ShippingMethodId")
                        .HasColumnType("uuid")
                        .HasColumnName("shipping_method_id");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time without time zone")
                        .HasColumnName("start_time");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("IsActive");

                    b.HasIndex("ShippingMethodId", "DayOfWeek");

                    b.ToTable("time_slot_templates", "shipping");
                });

            modelBuilder.Entity("ShippingService.Domain.Entities.ShippingMethod", b =>
                {
                    b.OwnsMany("ShippingService.Domain.ValueObjects.CostRule", "CostRules", b1 =>
                        {
                            b1.Property<Guid>("ShippingMethodId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");

                            b1.Property<bool>("IsActive")
                                .HasColumnType("boolean");

                            b1.Property<bool>("IsPercentage")
                                .HasColumnType("boolean");

                            b1.Property<int>("RuleType")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("ShippingMethodId", "Id");

                            b1.ToTable("shipping_methods", "shipping");

                            b1.ToJson("cost_rules");

                            b1.WithOwner()
                                .HasForeignKey("ShippingMethodId");
                        });

                    b.OwnsMany("ShippingService.Domain.ValueObjects.RestrictionRule", "RestrictionRules", b1 =>
                        {
                            b1.Property<Guid>("ShippingMethodId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            b1.Property<bool>("IsActive")
                                .HasColumnType("boolean");

                            b1.Property<int>("RuleType")
                                .HasColumnType("integer");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("ShippingMethodId", "Id");

                            b1.ToTable("shipping_methods", "shipping");

                            b1.ToJson("restriction_rules");

                            b1.WithOwner()
                                .HasForeignKey("ShippingMethodId");
                        });

                    b.Navigation("CostRules");

                    b.Navigation("RestrictionRules");
                });

            modelBuilder.Entity("ShippingService.Domain.Entities.TimeSlotBooking", b =>
                {
                    b.HasOne("ShippingService.Domain.Entities.TimeSlotTemplate", "TimeSlotTemplate")
                        .WithMany("Bookings")
                        .HasForeignKey("TimeSlotTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TimeSlotTemplate");
                });

            modelBuilder.Entity("ShippingService.Domain.Entities.TimeSlotTemplate", b =>
                {
                    b.HasOne("ShippingService.Domain.Entities.ShippingMethod", "ShippingMethod")
                        .WithMany("TimeSlotTemplates")
                        .HasForeignKey("ShippingMethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShippingMethod");
                });

            modelBuilder.Entity("ShippingService.Domain.Entities.ShippingMethod", b =>
                {
                    b.Navigation("TimeSlotTemplates");
                });

            modelBuilder.Entity("ShippingService.Domain.Entities.TimeSlotTemplate", b =>
                {
                    b.Navigation("Bookings");
                });
#pragma warning restore 612, 618
        }
    }
}
