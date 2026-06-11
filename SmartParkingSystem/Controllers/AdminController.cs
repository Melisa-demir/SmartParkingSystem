using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParkingSystem.Data;
using SmartParkingSystem.DTOs;

namespace SmartParkingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }

            [HttpGet("dashboard")]
            public async Task<ActionResult<DashboardDto>> GetDashboard()
            {
                var dashboard = new DashboardDto
                {
                    TotalUsers = await _context.Users.CountAsync(),
                    Totalreservations = await _context.Reservations.CountAsync(),

                    ActiveReservations = await _context.Reservations.CountAsync(x => x.Status == "Active"),

                    CompletedReservations = await _context.Reservations.CountAsync(x => x.Status == "Completed"),

                    TotalParkingSpots = await _context.ParkingSpots.CountAsync(x => x.IsOccupied),

                    EmptyParkingSpots = await _context.ParkingSpots.CountAsync(x => !x.IsOccupied)
                };

                return Ok(dashboard);
            }
        }
    }