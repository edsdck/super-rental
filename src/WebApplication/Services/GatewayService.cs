using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebApplication.Services
{
    public class GatewayService : IGatewayService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GatewayService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<GatewayResult<T>> Get<T>(string requestUri)
        {
            var client = _httpClientFactory.CreateClient(WebDefaults.HttpGatewayClientName);

            T requestResult;
            try
            {
                requestResult = await client.GetFromJsonAsync<T>(requestUri);
            }
            catch (HttpRequestException)
            {
                return new GatewayResult<T>
                {
                    ErrorMessage = "Nepavyko pasiekti duomenų dėl vidinių klaidų, pabandykite vėliau."
                };
            }

            return new GatewayResult<T>
            {
                Content = requestResult
            };
        }
        
        public async Task<BaseResult> Delete(string requestUri)
        {
            var client = _httpClientFactory.CreateClient(WebDefaults.HttpGatewayClientName);

            try
            {
                var requestResult = await client.DeleteAsync(requestUri);
            }
            catch (HttpRequestException)
            {
                return new BaseResult
                {
                    ErrorMessage = "Nepavyko ištrinti dėl vidinių klaidų, pabandykite vėliau."
                };
            }

            return new BaseResult();
        }
        
        public async Task<SendDataResult> Post<T>(string requestUri, T content)
        {
            var client = _httpClientFactory.CreateClient(WebDefaults.HttpGatewayClientName);
            
            HttpResponseMessage requestResult;
            try
            {
                requestResult = await client.PostAsJsonAsync(requestUri, content);
            }
            catch (HttpRequestException)
            {
                return new SendDataResult
                {
                    ErrorMessage = "Nepavyko išsaugoti pakeitimų dėl vidinių klaidų, pabandykite vėliau."
                };
            }

            return new SendDataResult
            {
                ErrorMessage = requestResult.IsSuccessStatusCode ? string.Empty : "WEB serveris grąžino klaidą.",
                LocationHeaderAbsolutePath = requestResult.Headers.Location?.AbsolutePath
            };
        }
        
        public async Task<SendDataResult> Put<T>(string requestUri, T content)
        {
            var client = _httpClientFactory.CreateClient(WebDefaults.HttpGatewayClientName);
            
            HttpResponseMessage requestResult;
            try
            {
                requestResult = await client.PutAsJsonAsync(requestUri, content);
            }
            catch (HttpRequestException)
            {
                return new SendDataResult
                {
                    ErrorMessage = "Nepavyko išsaugoti pakeitimų dėl vidinių klaidų, pabandykite vėliau."
                };
            }

            return new SendDataResult
            {
                ErrorMessage = requestResult.IsSuccessStatusCode ? string.Empty : "WEB serveris grąžino klaidą.",
                LocationHeaderAbsolutePath = requestResult.Headers.Location?.AbsolutePath
            };
        }
    }
    
    public interface IGatewayService
    {
        Task<GatewayResult<T>> Get<T>(string requestUri);

        Task<BaseResult> Delete(string requestUri);
        
        Task<SendDataResult> Post<T>(string requestUri, T content);

        Task<SendDataResult> Put<T>(string requestUri, T content);
    }

    public class BaseResult
    {
        public string ErrorMessage { get; init; }

        public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    }

    public class SendDataResult : BaseResult
    {
        public string LocationHeaderAbsolutePath { get; set; }
    }
    
    public class GatewayResult<T> : BaseResult
    {
        public T Content { get; init; }
    }
}