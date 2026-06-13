namespace ReservationsService.DTOs
{
    public class CreateReservationDto
    {
        public int UserId { get; set; }
        public int ParkingSpotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal HourlyPrice { get; set; }
    }
}
