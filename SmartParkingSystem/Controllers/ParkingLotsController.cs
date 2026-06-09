using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParkingSystem.Data;
using SmartParkingSystem.DTOs;
using SmartParkingSystem.Entities;
using Microsoft.AspNetCore.Authorization;

namespace SmartParkingSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingLotsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ParkingLotsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task <IActionResult> GetParkingLots()
        {

            var parkingLots = await _context.ParkingLots.ToListAsync();
            return Ok(parkingLots);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetParkingLot(int id)
        {
            var parkingLot = await _context.ParkingLots.FindAsync(id);

            if (parkingLot == null)
                return NotFound("Parking lot not found.");

            return Ok(parkingLot);
        }

        [HttpPost]
        public async Task<IActionResult> CreateParkingLot(CreateParkingLotDto dto)
        {
            var parkingLot = new ParkingLot
            {
                Name = dto.Name,
                Address = dto.Address,
                TotalCapacity = dto.TotalCapacity
            };

            _context.ParkingLots.Add(parkingLot);
            await _context.SaveChangesAsync();

            return Ok(parkingLot);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParkingLot(int id, ParkingLot parkingLot)
        {
            var existingParkingLot = await _context.ParkingLots.FindAsync(id);

            if(existingParkingLot == null)
            {
                return NotFound("Parking lot not found");
            }

            existingParkingLot.Name = parkingLot.Name;
            existingParkingLot.Address = parkingLot.Address;
            existingParkingLot.TotalCapacity = parkingLot.TotalCapacity;

            return Ok(parkingLot);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingLot(int id )
        {
            var parkingLot = await _context.ParkingLots.FindAsync(id);
            if(parkingLot == null)
                return NotFound("Parking lot not found");

            _context.ParkingLots.Remove(parkingLot);
            await _context.SaveChangesAsync();

            return Ok("Parking lot deleted succesfully");
            
        }
    }
}
