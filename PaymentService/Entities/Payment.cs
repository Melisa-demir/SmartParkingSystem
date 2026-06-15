namespace PaymentService.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "Paid";
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
