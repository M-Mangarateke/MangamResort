using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManResort.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RedeemDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Adults = table.Column<int>(type: "int", nullable: false),
                    Kids = table.Column<int>(type: "int", nullable: false),
                    HolidayAdults = table.Column<int>(type: "int", nullable: false),
                    HolidayKids = table.Column<int>(type: "int", nullable: false),
                    CoolerBoxes = table.Column<int>(type: "int", nullable: false),
                    BusinessEntrance = table.Column<int>(type: "int", nullable: false),
                    BusinessElectricEntrance = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QRCodePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRedeemed = table.Column<bool>(type: "bit", nullable: false),
                    PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.TicketId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
