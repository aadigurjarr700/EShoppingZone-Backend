using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileService.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "ProfileId", "About", "DateOfBirth", "EmailId", "FullName", "Gender", "Image", "MobileNumber", "Password", "Role" },
                values: new object[] { 999, "Platform Administrator", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@eshoppingzone.com", "System Admin", "Other", "admin.png", 1234567890L, "$2a$11$lFiBj5L0IRLCdKtQx7rzGukI0NDPMnQnH3dJRwF/2IBI.1ILOhS6S", "ADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "ProfileId",
                keyValue: 999);
        }
    }
}
