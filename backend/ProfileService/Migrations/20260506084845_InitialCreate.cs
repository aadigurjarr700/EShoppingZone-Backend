using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProfileService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    ProfileId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false),
                    EmailId = table.Column<string>(type: "text", nullable: false),
                    MobileNumber = table.Column<long>(type: "bigint", nullable: false),
                    About = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.ProfileId);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HouseNumber = table.Column<int>(type: "integer", nullable: false),
                    StreetName = table.Column<string>(type: "text", nullable: false),
                    ColonyName = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    Pincode = table.Column<int>(type: "integer", nullable: false),
                    ProfileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Addresses_UserProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "ProfileId", "About", "DateOfBirth", "EmailId", "FullName", "Gender", "Image", "MobileNumber", "Password", "Role" },
                values: new object[] { 999, "Platform Administrator", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "aadiadmin123@gmail.com", "AadiAdmin", "Male", "admin.png", 1234567890L, "$2a$11$ZBFUxOHiZ5y0fQ0K8.oydOXIvrSmp8Coi8eZcNowkHc75Je3pTCxa", "ADMIN" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ProfileId",
                table: "Addresses",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_EmailId",
                table: "UserProfiles",
                column: "EmailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_MobileNumber",
                table: "UserProfiles",
                column: "MobileNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
