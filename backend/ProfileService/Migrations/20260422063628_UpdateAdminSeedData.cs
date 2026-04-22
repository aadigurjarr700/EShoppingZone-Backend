using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserProfiles",
                keyColumn: "ProfileId",
                keyValue: 999,
                columns: new[] { "EmailId", "FullName", "Gender", "Password" },
                values: new object[] { "aadiadmin123@gmail.com", "AadiAdmin", "Male", "$2a$11$prX7A0EOFv9dtt2ih7qsbukXjP44X/adAcFfNYvOe2IdcXCXysHMO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserProfiles",
                keyColumn: "ProfileId",
                keyValue: 999,
                columns: new[] { "EmailId", "FullName", "Gender", "Password" },
                values: new object[] { "admin@eshoppingzone.com", "System Admin", "Other", "$2a$11$lFiBj5L0IRLCdKtQx7rzGukI0NDPMnQnH3dJRwF/2IBI.1ILOhS6S" });
        }
    }
}
