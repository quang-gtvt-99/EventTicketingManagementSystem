using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventTicketingManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingCodeForBookingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookingCode",
                table: "Bookings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingCode",
                table: "Bookings");
        }
    }
}
