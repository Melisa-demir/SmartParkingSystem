using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.DTOs;
using PaymentService.Entities;
using PaymentService.Services;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentDbContext _context;
        private readonly ReservationServicesClient _reservationServicesClient;

        public PaymentsController(PaymentDbContext context, ReservationServicesClient reservationServicesClient)
        {
            _context = context;
            _reservationServicesClient = reservationServicesClient;
        }

        [HttpGet]
        public async Task <IActionResult> GetPayments()
        {
            var payment = await _context.Payments.ToListAsync();
            return Ok(payment);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentsById (int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if(payment == null)
            {
                return NotFound("Payment not found");
            }

            return Ok(payment);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(CreatePaymentDto dto)
        {
            var reservation = await _reservationServicesClient.GetReservationById(dto.ReservationId);

            if(reservation.Status != "Active")
            {
                return BadRequest("Only active reservations can be paid");
            }

            var hasPaidPayment = await _context.Payments
                .AnyAsync(x => x.ReservationId == dto.ReservationId && x.PaymentStatus == "Paid");

            if(hasPaidPayment)
            {
                return BadRequest("Reservation already paid");
            }

            var payment = new Payment
            {
                ReservationId = dto.ReservationId,
                Amount = reservation.TotalPrice,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "Paid",
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(payment);
        }

    }
}
