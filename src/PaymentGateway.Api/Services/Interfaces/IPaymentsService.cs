using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services.Interfaces
{
    public interface IPaymentsService
    {
        PostPaymentResponse? GetPayment(Guid id);
        public Task<PostPaymentResponse?> ProcessPaymentAsync(PostPaymentRequest request);
    }
}
