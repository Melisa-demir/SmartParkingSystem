using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartParkingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddHourlyPriceToParkingSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingLots_ParkingLots_ParkingLotId",
                table: "ParkingLots");

            migrationBuilder.DropIndex(
                name: "IX_ParkingLots_ParkingLotId",
                table: "ParkingLots");

            migrationBuilder.DropColumn(
                name: "ParkingLotId",
                table: "ParkingLots");

            migrationBuilder.AddColumn<decimal>(
                name: "HourlyPrice",
                table: "ParkingSpots",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourlyPrice",
                table: "ParkingSpots");

            migrationBuilder.AddColumn<int>(
                name: "ParkingLotId",
                table: "ParkingLots",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParkingLots_ParkingLotId",
                table: "ParkingLots",
                column: "ParkingLotId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingLots_ParkingLots_ParkingLotId",
                table: "ParkingLots",
                column: "ParkingLotId",
                principalTable: "ParkingLots",
                principalColumn: "Id");
        }
    }
}
