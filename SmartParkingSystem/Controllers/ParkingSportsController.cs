using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParkingSystem.Data;
using SmartParkingSystem.DTOs;
using SmartParkingSystem.Entities;

namespace SmartParkingSystem.Controllers
{
    [Route("api/controller")]
    [ApiController]
    public class ParkingSportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ParkingSportsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetParkingSpot ()
        {
            var parkingSpots = await _context.ParkingSpots
                .Include(x => x.ParkingLot)
                .ToListAsync();

            return Ok(parkingSpots);
        }

        [HttpPost]
        public async Task<IActionResult> CreateParkingSpot (CreateParkingSpotDto dto)
        {
            var parkingLotExists = await _context.ParkingLots
                .AnyAsync(x => x.Id == dto.ParkingLotId);

            if (!parkingLotExists)
                return BadRequest("Parking lot is found");

            var parkingSpot = new ParkingSpot
            {
                SpotNumber = dto.SpotNumber,
                IsOccupied = dto.IsOccupied,
                ParkingLotId = dto.ParkingLotId
            };
            _context.ParkingSpots.Add(parkingSpot);
            await _context.SaveChangesAsync();

            return Ok(parkingSpot);

        }
    }
}
