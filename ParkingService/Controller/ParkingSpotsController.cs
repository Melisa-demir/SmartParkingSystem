using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingService.Data;
using ParkingService.DTOs;
using ParkingService.Entities;

namespace ParkingService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingSpotsController : ControllerBase
    {
        private readonly ParkingDbContext _context;
        public ParkingSpotsController(ParkingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetParkingSpots()
        {
            var parkingSpots = await _context.ParkingSpots
                .Select(x => new
                {
                    x.Id,
                    x.SpotNumber,
                    x.IsOccupied,
                    x.ParkingLotId,
                    x.HourlyPrice
                })
                .ToListAsync();

            return Ok(parkingSpots);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetParkingSpot(int id)
        {
            var parkingSpot = await _context.ParkingSpots
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.SpotNumber,
                    x.IsOccupied,
                    x.ParkingLotId,
                    x.HourlyPrice
                })
                .FirstOrDefaultAsync();

            if (parkingSpot == null)
            {
                return NotFound("Parking spot not found.");
            }

            return Ok(parkingSpot);
        }
        [HttpPost]
        public async Task<IActionResult> CreateParkingLot(CreateParkingSpotDto dto)
        {
            var parkingLotExists = await _context.ParkingLots
                .AnyAsync(x => x.Id == dto.ParkingLotId);

            if(!parkingLotExists)
            {
                return BadRequest("Parking lot not found");
            }

            var parkingSpot = new ParkingSpot
            {
                SpotNumber = dto.SpotNumber,
                IsOccupied = dto.IsOccupied,
                ParkingLotId = dto.ParkingLotId,
                HourlyPrice = dto.HourlyPrice
            };

            _context.ParkingSpots.Add(parkingSpot);
            await _context.SaveChangesAsync();

            return Ok(parkingSpot);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParkingLot(int id, UpdateParkingSpotDto dto)
        {
            var existingParkingSpot = await _context.ParkingSpots.FindAsync(id);

            if(existingParkingSpot == null)
            {
                return NotFound("Parking spot not found");
            }

            var parkingLotExists = await _context.ParkingLots
                .AnyAsync(x => x.Id == dto.ParkingLotId);

            if(!parkingLotExists)
            {
                return BadRequest("Parking lot not found");
            }

            existingParkingSpot.SpotNumber = dto.SpotNumber;
            existingParkingSpot.IsOccupied = dto.IsOccupied;
            existingParkingSpot.ParkingLotId = dto.ParkingLotId;
            existingParkingSpot.HourlyPrice = dto.HourlyPrice;

            await _context.SaveChangesAsync();
            return Ok(existingParkingSpot);
        }

        [HttpPut("{id}/toggle-status")]
        public async Task<IActionResult> ToggleParkingSpotStatus(int id)
        {
            var parkingSpot = await _context.ParkingSpots.FindAsync(id);

            if(parkingSpot == null)
            {
                return NotFound("Parking spot not found");
            }

            parkingSpot.IsOccupied = !parkingSpot.IsOccupied;

            await _context.SaveChangesAsync();
            return Ok(parkingSpot);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingSpot(int id)
        {
            var parkingSpot = await _context.ParkingSpots.FindAsync(id);
            
            if(parkingSpot == null)
            {
                return NotFound("Parking Spot is not found");
            }

            _context.ParkingSpots.Remove(parkingSpot);

            await _context.SaveChangesAsync();

            return Ok("Parking Spot deleted succesfully");
        }
    }
}
