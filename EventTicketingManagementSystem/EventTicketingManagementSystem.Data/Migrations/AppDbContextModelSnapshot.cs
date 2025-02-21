﻿// <auto-generated />
using System;
using System.Diagnostics.CodeAnalysis;
using EventTicketingManagementSystem.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EventTicketingManagementSystem.Migrations
{
    [ExcludeFromCodeCoverage]
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EventId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Subtotal")
                        .HasColumnType("numeric");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("numeric");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("ExpiryDate")
                        .HasDatabaseName("IX_Bookings_ExpiryDate");

                    b.HasIndex("Status")
                        .HasDatabaseName("IX_Bookings_Status");

                    b.HasIndex("UserId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ArtistInfo")
                        .HasColumnType("text");

                    b.Property<string>("Category")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ImageUrls")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("RemainingTickets")
                        .HasColumnType("integer");

                    b.Property<decimal?>("SeatPrice")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<int?>("TotalTickets")
                        .HasColumnType("integer");

                    b.Property<string>("TrailerUrls")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("VenueAddress")
                        .HasColumnType("text");

                    b.Property<string>("VenueName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Category")
                        .HasDatabaseName("IX_Events_Category");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("Name")
                        .HasDatabaseName("IX_Events_Name");

                    b.HasIndex("StartDate")
                        .HasDatabaseName("IX_Events_StartDate");

                    b.HasIndex("Status")
                        .HasDatabaseName("IX_Events_Status");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<int>("BookingId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("PaymentDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("RefundDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TransactionId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("TransactionId")
                        .IsUnique()
                        .HasDatabaseName("IX_Payments_TransactionId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Seat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("BookingId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EventId")
                        .HasColumnType("integer");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<string>("Row")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("EventId");

                    b.HasIndex("Row")
                        .HasDatabaseName("IX_Seats_Row");

                    b.HasIndex("Status")
                        .HasDatabaseName("IX_Seats_Status");

                    b.ToTable("Seats");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BookingId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("ConfirmedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ReservedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("SeatId")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TicketNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("SeatId")
                        .IsUnique();

                    b.HasIndex("TicketNumber")
                        .IsUnique()
                        .HasDatabaseName("IX_Tickets_TicketNumber");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("IX_Users_Email");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.UserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("AssignedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("AssignedBy")
                        .HasColumnType("integer");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId")
                        .HasDatabaseName("IX_UserRoles_UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Booking", b =>
                {
                    b.HasOne("EventTicketingMananagementSystem.Core.Models.Event", "Event")
                        .WithMany("Bookings")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EventTicketingMananagementSystem.Core.Models.User", "User")
                        .WithMany("Bookings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("User");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Event", b =>
                {
                    b.HasOne("EventTicketingMananagementSystem.Core.Models.User", "User")
                        .WithMany("Events")
                        .HasForeignKey("CreatedBy");

                    b.Navigation("User");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Payment", b =>
                {
                    b.HasOne("EventTicketingMananagementSystem.Core.Models.Booking", "Booking")
                        .WithMany("Payments")
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Seat", b =>
                {
                    b.HasOne("EventTicketingMananagementSystem.Core.Models.Booking", "Booking")
                        .WithMany("Seats")
                        .HasForeignKey("BookingId");

                    b.HasOne("EventTicketingMananagementSystem.Core.Models.Event", "Event")
                        .WithMany("Seats")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Ticket", b =>
                {
                    b.HasOne("EventTicketingMananagementSystem.Core.Models.Booking", "Booking")
                        .WithMany("Tickets")
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EventTicketingMananagementSystem.Core.Models.Seat", "Seat")
                        .WithOne("Ticket")
                        .HasForeignKey("EventTicketingMananagementSystem.Core.Models.Ticket", "SeatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("Seat");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.UserRole", b =>
                {
                    b.HasOne("EventTicketingMananagementSystem.Core.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EventTicketingMananagementSystem.Core.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Booking", b =>
                {
                    b.Navigation("Payments");

                    b.Navigation("Seats");

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Event", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("Seats");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Role", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.Seat", b =>
                {
                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("EventTicketingMananagementSystem.Core.Models.User", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("Events");

                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
