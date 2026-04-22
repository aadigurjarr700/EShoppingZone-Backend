using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ModeOfPayment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Address_CustomerId = table.Column<int>(type: "int", nullable: true),
                    Address_FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_FlatNumber = table.Column<int>(type: "int", nullable: true),
                    Address_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_Pincode = table.Column<int>(type: "int", nullable: true),
                    Address_State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Product_ProductId = table.Column<int>(type: "int", nullable: true),
                    Product_ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
