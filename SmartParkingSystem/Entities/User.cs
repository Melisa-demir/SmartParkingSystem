using System.Reflection.Metadata;

namespace SmartParkingSystem.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
        public string Role { get; set; }
    }
}
