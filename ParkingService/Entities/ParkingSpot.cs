namespace ParkingService.Entities
{
    public class ParkingSpot
    {
        public int Id { get; set; }
        public string SpotNumber { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public int ParkingLotId { get; set; }
        public ParkingLot? ParkingLot { get; set; }
        public decimal HourlyPrice  { get; set; }
    }
}
