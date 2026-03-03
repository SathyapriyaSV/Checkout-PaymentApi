using PaymentGateway.Api.Clients.Models;

namespace PaymentGateway.Api.Interfaces
{
    public interface IBankHttpClient
    {
        Task<ApiResponse<TResponse?>> PostAsync<TRequest, TResponse>(string url, TRequest request);
    }   
}
