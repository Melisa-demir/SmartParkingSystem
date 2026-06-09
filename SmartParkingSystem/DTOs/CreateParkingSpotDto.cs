namespace SmartParkingSystem.DTOs
{
    public class CreateParkingSpotDto
    {
        public string SpotNumber { get; set; }
        public bool IsOccupied { get; set; }
        public int ParkingLotId { get; set; }
    }
}
