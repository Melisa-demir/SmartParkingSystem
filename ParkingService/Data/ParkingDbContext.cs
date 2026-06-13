using Microsoft.EntityFrameworkCore;
using ParkingService.Entities;

namespace ParkingService.Data
{
    public class ParkingDbContext : DbContext
    {
        public ParkingDbContext(DbContextOptions<ParkingDbContext> options) : base(options)
        {
        }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<ParkingLot> ParkingLots { get; set; }
    }
}
