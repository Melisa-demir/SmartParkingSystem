namespace ParkingService.DTOs
{
    public class CreateParkingLotDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int TotalCapacity { get; set; }
    }
}
