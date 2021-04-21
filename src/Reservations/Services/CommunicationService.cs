using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Reservations.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CommunicationService(IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<bool> GetIfRentalExists(int rentalId, string bearerToken)
        {
            var rentalServiceUrl = _configuration.GetSection("ServiceUrls")["rentals"];
            
            var httpClient = _httpClientFactory.CreateClient();
            
            _ = AuthenticationHeaderValue.TryParse(bearerToken, out var headerValue);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Head,
                RequestUri = new Uri($"{rentalServiceUrl}/rentals/{rentalId}")
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", headerValue?.Parameter);
            
            var result = await httpClient.SendAsync(requestMessage);

            return result.StatusCode == HttpStatusCode.OK;
        }
    }
}