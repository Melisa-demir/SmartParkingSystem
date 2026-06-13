using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingService.Data;
using ParkingService.DTOs;
using ParkingService.Entities;

namespace ParkingService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingLotsController : ControllerBase
    {
        private readonly ParkingDbContext _context;
        public ParkingLotsController (ParkingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task <IActionResult> GetParkingLots()
        {
            var parkingLots = await _context.ParkingLots
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Address,
                    x.TotalCapacity
                })
                .ToListAsync();
                 return Ok(parkingLots);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetParkingLot(int id)
        {
            var parkingLot = await _context.ParkingLots
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Address,
                    x.TotalCapacity
                })
                .FirstOrDefaultAsync();

            if (parkingLot == null)
                return NotFound("Parking lot not found.");

            return Ok(parkingLot);
        }

        [HttpPost]
        public async Task<IActionResult> CreateParkigLots (CreateParkingLotDto dto)
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
        public async Task<IActionResult> UpdateParkingLot(int id, UpdateParkingLotDto dto)
        {
            var existingParkingLot = await _context.ParkingLots.FindAsync(id);

            if (existingParkingLot == null)
            { 
                return NotFound("Parking lot is null");
            }

            existingParkingLot.Name = dto.Name;
            existingParkingLot.Address = dto.Address;
            existingParkingLot.TotalCapacity = dto.TotalCapacity;

            await _context.SaveChangesAsync();
            return Ok(existingParkingLot);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingLot(int id)
        {
            var parkingLot = await _context.ParkingLots.FindAsync(id);
            if (parkingLot == null)
            {
                return NotFound("Parking Lot not found");
            }

            _context.ParkingLots.Remove(parkingLot);
            await _context.SaveChangesAsync();

            return Ok("Parking Lot deleted succesfully");
        }
}}
