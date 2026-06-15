using ReservationsService.DTOs;

namespace ReservationsService.Services
{
    public class ParkingServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ParkingServiceClient (HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ParkingSpotDto?> GetParkingSpotById(int parkingSpotId)
        {
            var parkingServiceUrl = _configuration["ServiceUrls:ParkingService"];

            var parkingSpot = await _httpClient.GetFromJsonAsync<ParkingSpotDto>(
                $"{parkingServiceUrl}/api/ParkingSpots/{parkingSpotId}");

            return parkingSpot;
        }

        public async Task<bool> OccupyParkingSpot (int parkingSpotId)
        {
            var parkingServiceUrl = _configuration["ServiceUrls:ParkingService"];

            var response = await _httpClient.PutAsync($"{parkingServiceUrl}/api/ParkingSpots/{parkingSpotId}/occupy", null);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReleaseParkingSpot (int parkingSpotId)
        {
            var parkingServiceUrl = _configuration["ServiceUrls:ParkingService"];

            var response = await _httpClient.PutAsync($"{parkingServiceUrl}/api/ParkingSpots/{parkingSpotId}/release",null);

            return response.IsSuccessStatusCode;
        }

    }
}
