namespace ReservationsService.DTOs
{
    public class ParkingSpotDto
    {
        public int Id { get; set; }
        public string SpotNumber { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public int ParkingLotId { get; set; }
        public decimal HourlyPrice { get; set; }
    }
}
