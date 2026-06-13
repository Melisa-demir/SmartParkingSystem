namespace ParkingService.DTOs
{
    public class UpdateParkingSpotDto
    {
        public string SpotNumber { get; set; }  = string.Empty;
        public bool IsOccupied { get; set; }
        public int ParkingLotId { get; set; }
        public decimal HourlyPrice { get; set; }
    }
}
