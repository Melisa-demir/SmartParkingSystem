using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParkingSystem.Data;
using SmartParkingSystem.DTOs;
using SmartParkingSystem.Entities;
using Microsoft.AspNetCore.Authorization;

namespace SmartParkingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController (AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var emailExists = await _context.Users.AnyAsync(x => x.Email == dto.Email);

            if (emailExists)
                return BadRequest("Email already exist");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.Password,
                CreatedDate = DateTime.Now,
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult>UpdateUser(int id, CreateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.PasswordHash = dto.Password;
            user.Role = dto.Role;

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted is successfully");

        }
    }
}
