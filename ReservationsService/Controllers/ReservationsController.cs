using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationsService.Data;
using ReservationsService.DTOs;
using ReservationsService.Entities;
using ReservationsService.Services;

namespace ReservationsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationDbContext _context;
        private readonly ParkingServiceClient _parkingServiceClient;
        public ReservationsController(ReservationDbContext context, ParkingServiceClient parkingServiceClient)
        {
            _context = context;
            _parkingServiceClient = parkingServiceClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await _context.Reservations.ToListAsync();
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(CreateReservationDto dto)
        {
            if(dto.EndTime <= dto.StartTime)
            {
                return BadRequest("End time must be greater than start time");
            }

            var parkingSpot = await _parkingServiceClient.GetParkingSpotById(dto.ParkingSpotId);

            if(parkingSpot == null)
            {
                return BadRequest("Parking spot not found");
            }

            if(parkingSpot.IsOccupied)
            {
                return BadRequest("Parking spot is already occupied");
            }

            var hasActiveReservation = await _context.Reservations
                .AnyAsync(x => x.UserId == dto.UserId && x.Status == "Active");

            if(hasActiveReservation)
            {
                return BadRequest("User already has an active reservation");
            }

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
            await _context.SaveChangesAsync();

            var occupied = await _parkingServiceClient.OccupyParkingSpot(dto.ParkingSpotId);

            if(!occupied)
            {
                return BadRequest("Reservation created but parking spot could not be occupied");
            }

            return Ok(reservation);
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound("Reservation not found.");
            }

            var released = await _parkingServiceClient.ReleaseParkingSpot(reservation.ParkingSpotId);

            if (!released)
            {
                return BadRequest("Parking spot could not be released.");
            }

            if (reservation.Status != "Completed")
            {
                reservation.Status = "Completed";
                await _context.SaveChangesAsync();
            }

            return Ok("Reservation completed successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResevation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if(reservation == null)
            {
                return NotFound("Reservation not found");
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok("Reservation is deleted successfully");
        }

    }
}
