namespace SmartParkingSystem.Entities
{
    public class ParkingLot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int TotalCapacity { get; set; }
        public ICollection<ParkingLot> ParkingSpots { get; set; }
    }
}
