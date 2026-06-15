namespace PaymentService.DTOs
{
    public class CreatePaymentDto
    {
        public int ReservationId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
