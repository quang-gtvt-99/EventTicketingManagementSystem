using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventTicketingManagementSystem.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class changeColEventandBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "TrailerUrls",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrailerUrls",
                table: "Events");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "Bookings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
