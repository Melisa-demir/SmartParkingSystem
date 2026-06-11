namespace SmartParkingSystem.DTOs
{
    public class DashboardDto
    {
        public int TotalUsers { get; set; }
        public int Totalreservations { get; set; }
        public int ActiveReservations { get; set; }
        public int CompletedReservations { get; set; }
        public int TotalParkingSpots { get; set; }
        public int OccupiedParkingSpots { get; set; }
        public int EmptyParkingSpots { get; set; }
    }
}
