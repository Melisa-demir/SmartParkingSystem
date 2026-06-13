namespace ParkingService.DTOs
{
    public class CreateParkingSpotDto
    {
        public string SpotNumber { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public int ParkingLotId { get; set; }
        public decimal HourlyPrice { get; set; }
    }
}
