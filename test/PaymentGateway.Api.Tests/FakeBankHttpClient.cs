using PaymentGateway.Api.External.Interfaces;
using PaymentGateway.Api.External.Models.Response;

namespace PaymentGateway.Api.Tests
{
    public class FakeBankHttpClient : IBankHttpClient
    {
        public Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request)
        {
            var response = new ApiResponse<TResponse>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = (TResponse)(object)new BankPaymentResponse
                {
                    Authorized = true
                }
            };

            return Task.FromResult(response);
        }
    }
}
