using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParkingSystem.Data;
using SmartParkingSystem.DTOs;
using SmartParkingSystem.Entities;

namespace SmartParkingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await _context.Reservations
                .Include(x => x.User)
                .Include(x => x.ParkingSpot)
                .ToListAsync();

            return Ok(reservations);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(CreateReservationDto dto)
        {
            var userExists = await _context.Users.AnyAsync(x => x.Id == dto.UserId);

            if (!userExists)
                return BadRequest("User not found");

            var parkingSpot = await _context.ParkingSpots
                .FirstOrDefaultAsync(x => x.Id == dto.ParkingSpotId);

            if (parkingSpot == null)
                return BadRequest("Parking spot not found");

            if (parkingSpot.IsOccupied)
                return BadRequest("Parking spot is already occupied");


            if (dto.EndTime <= dto.StartTime)
                return BadRequest("End time must be greater than start time");

            var totalHours = (decimal)(dto.EndTime - dto.StartTime).TotalHours;
            var totalPrice = totalHours * parkingSpot.HourlyPrice;

            var reservation = new Reservation
            {
                UserId = dto.UserId,
                ParkingSpotId = dto.ParkingSpotId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                TotalPrice = totalPrice,
                Status = "Active"
            };

            _context.Reservations.Add(reservation);
            parkingSpot.IsOccupied = true;
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }
    }
}
