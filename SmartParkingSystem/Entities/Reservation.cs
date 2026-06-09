namespace SmartParkingSystem.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int ParkingSpotId { get; set; }
        public ParkingSpot? ParkingSpot { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } 
    }
}
