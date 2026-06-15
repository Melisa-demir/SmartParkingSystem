using PaymentService.DTOs;

namespace PaymentService.Services
{
    public class ReservationServicesClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ReservationServicesClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ReservationDto?> GetReservationById(int reservationId)
        {
            var reservationServiceUrl = _configuration["ServiceUrls:ReservationsService"];

            var response = await _httpClient.GetAsync(
                $"{reservationServiceUrl}/api/Reservations/{reservationId}");

            if(!response.IsSuccessStatusCode)
            {
                return null;
            }

            var reservation = await response.Content.ReadFromJsonAsync<ReservationDto>();

            return reservation;
        }
    }
}
