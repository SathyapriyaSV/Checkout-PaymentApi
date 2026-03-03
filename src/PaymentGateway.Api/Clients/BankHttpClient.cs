using PaymentGateway.Api.Clients.Models;
using PaymentGateway.Api.Interfaces;

namespace PaymentGateway.Api.Clients
{
    public class BankHttpClient(HttpClient httpClient) : IBankHttpClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request)
        {
            var httpResponse = await _httpClient.PostAsJsonAsync(url, request);

            var statusCode = httpResponse.StatusCode;

            if (httpResponse.IsSuccessStatusCode)
            {
                var data = await httpResponse.Content.ReadFromJsonAsync<TResponse>();

                return new ApiResponse<TResponse>
                {
                    StatusCode = statusCode,
                    Data = data
                };
            }

            var errorContent = await httpResponse.Content.ReadAsStringAsync();

            return new ApiResponse<TResponse>
            {
                StatusCode = statusCode,
                ErrorContent = errorContent
            };
        }
    }
}
