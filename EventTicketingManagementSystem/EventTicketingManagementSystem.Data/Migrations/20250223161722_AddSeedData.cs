using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventTicketingManagementSystem.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 11, 17, 2, 59, 342, DateTimeKind.Utc).AddTicks(2600), "Administrator role", "Admin", null },
                    { 2, new DateTime(2025, 2, 11, 17, 2, 59, 342, DateTimeKind.Utc).AddTicks(2600), "Regular user role", "User", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "LastLoginAt", "PasswordHash", "PhoneNumber", "Status", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2025, 2, 11, 17, 2, 59, 342, DateTimeKind.Utc).AddTicks(2600), "admin@example.com", "Admin User", null, "$2a$11$cUybrThoNHRmDh88vkV8o.pRDnauBGuGZDyvp7yvQjvFAxWjuC9xm", "1234567890", "Active", null });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt", "AssignedBy", "Id" },
                values: new object[] { 1, 1, new DateTime(2025, 2, 11, 17, 2, 59, 342, DateTimeKind.Utc).AddTicks(2600), null, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
