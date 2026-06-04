namespace SmartParkingSystem.Entities
{
    public class ParkingSpot
    {
        public int Id { get; set; }
        public string SpotNumber { get; set; }
        public bool IsOccupied { get; set; }
        public int ParkingLotId { get; set; }
        public ParkingLot ParkingLot { get; set; }
    }
}
