namespace ParkingService.Entities
{
    public class ParkingLot
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int TotalCapacity { get; set; }
        public ICollection<ParkingSpot>? ParkingSpots { get; set; }
    }
}
