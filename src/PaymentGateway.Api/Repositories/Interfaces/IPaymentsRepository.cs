using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Repositories.Interfaces
{
    public interface IPaymentsRepository
    {
        void Add(Payments payment);
        Payments? Get(Guid id);
    }
}
