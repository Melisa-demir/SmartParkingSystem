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
    public class ReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ParkingHub> _hubContext;

        public ReservationsController(AppDbContext context, IHubContext<ParkingHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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

            await _hubContext.Clients.All.SendAsync("ParkingSpotUpdated", new
            {
                parkingSpotId = parkingSpot.Id,
                spotNumber = parkingSpot.SpotNumber,
                isOccupied = parkingSpot.IsOccupied
            });

            return Ok(reservation);
        }


        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(x => x.ParkingSpot)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (reservation == null)
                return NotFound("Reservation not found");

            if (reservation.Status == "Completed")
                return BadRequest("Reservation is already completed.");

            reservation.Status = "Completed";

            if (reservation.ParkingSpot != null)
                reservation.ParkingSpot.IsOccupied = false;

            await _context.SaveChangesAsync();
            return Ok("Reservation completed successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation (int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                return NotFound("Reservation not found");

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok("Reservation deleted succesfully");
        }
        
    }
}
