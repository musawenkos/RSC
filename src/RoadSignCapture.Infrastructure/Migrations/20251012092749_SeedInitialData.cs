using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoadSignCapture.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Company",
                columns: new[] { "CompanyId", "CompanyName", "ContactNumber", "Created", "FullAddress", "Updated" },
                values: new object[] { 1, "MJNEXUS SYSTEMS", "0732347796", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Head Office", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "SysAdmin" },
                    { 2, "Designer" },
                    { 3, "Client" },
                    { 4, "Viewer" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Email", "CompanyId", "Created", "DisplayName", "Updated" },
                values: new object[] { "ndlelamusa1st@gmail.com", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "M Ndlela", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "RoleId", "UserEmail" },
                values: new object[] { 1, "ndlelamusa1st@gmail.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "UserRole",
                keyColumns: new[] { "RoleId", "UserEmail" },
                keyValues: new object[] { 1, "ndlelamusa1st@gmail.com" });

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Email",
                keyValue: "ndlelamusa1st@gmail.com");

            migrationBuilder.DeleteData(
                table: "Company",
                keyColumn: "CompanyId",
                keyValue: 1);
        }
    }
}
