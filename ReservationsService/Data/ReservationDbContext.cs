using Microsoft.EntityFrameworkCore;
using ReservationsService.Entities;

namespace ReservationsService.Data
{
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions<ReservationDbContext> options) : base(options)
        {
        }
        
        public DbSet<Reservation> Reservations { get; set; }
    }
}
