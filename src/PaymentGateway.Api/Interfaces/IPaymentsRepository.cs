using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces
{
    public interface IPaymentsRepository
    {
        void Add(PostPaymentResponse payment);
        PostPaymentResponse? Get(Guid id);
    }
}
