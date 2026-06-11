using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParkingSystem.Data;
using SmartParkingSystem.DTOs;
using SmartParkingSystem.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmartParkingSystem.Hubs;

namespace SmartParkingSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingSportsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ParkingHub> _hubContext;

        public ParkingSportsController(AppDbContext context, IHubContext<ParkingHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetParkingSpot()
        {
            var parkingSpots = await _context.ParkingSpots
                .Include(x => x.ParkingLot)
                .ToListAsync();

            return Ok(parkingSpots);
        }

        [HttpPost]
        public async Task<IActionResult> CreateParkingSpot(CreateParkingSpotDto dto)
        {
            var parkingLotExists = await _context.ParkingLots
                .AnyAsync(x => x.Id == dto.ParkingLotId);

            if (!parkingLotExists)
                return BadRequest("Parking lot is found");

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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetParkingSpotId(int id)
        {
            var parkingSpot = await _context.ParkingSpots
                .Include(x => x.ParkingLot)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (parkingSpot is null)
                return NotFound("ParkingSpot is bad request");

            return Ok(parkingSpot);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParkingSpot(int id, CreateParkingSpotDto dto)
        {
            var parkingSpot = await _context.ParkingSpots.FindAsync(id);

            if (parkingSpot is null)
                return NotFound("Parking Spot is not found");

            var parkingLotExists = await _context.ParkingLots
                .AnyAsync(x => x.Id == dto.ParkingLotId);

            if (!parkingLotExists)
                return BadRequest("Parking lot not found");

            parkingSpot.SpotNumber = dto.SpotNumber;
            parkingSpot.IsOccupied = dto.IsOccupied;
            parkingSpot.ParkingLotId = dto.ParkingLotId;
            parkingSpot.HourlyPrice = dto.HourlyPrice;

            await _context.SaveChangesAsync();

            return Ok(parkingSpot);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingSpot (int id)
        {
            var parkingSpot = await _context.ParkingSpots.FindAsync(id);

            if (parkingSpot is null)
                return NotFound("Parking Spots is not found");

            _context.ParkingSpots.Remove(parkingSpot);
            await _context.SaveChangesAsync();

            return Ok("Parking Spot is remove");

        }

        [HttpPut("{id}/toggle-status")]
        public async Task <IActionResult> ToggleParkingSpotStatus (int id)
        {
            var parkingSpot = await _context.ParkingSpots.FindAsync(id);

            if (parkingSpot == null)
                return NotFound("Parking spot not found");

            parkingSpot.IsOccupied = !parkingSpot.IsOccupied;

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ParkingSpotUpdated", new
            {
                parkingSpotId = parkingSpot.Id,
                spotNumber = parkingSpot.SpotNumber,
                isOccupied = parkingSpot.IsOccupied
            });

            return Ok(parkingSpot);
        }
    }
}
