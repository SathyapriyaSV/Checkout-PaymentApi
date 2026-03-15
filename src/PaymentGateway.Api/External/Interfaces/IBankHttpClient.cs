using PaymentGateway.Api.External.Models.Response;

namespace PaymentGateway.Api.External.Interfaces
{
    public interface IBankHttpClient
    {
        Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request);
    }   
}
