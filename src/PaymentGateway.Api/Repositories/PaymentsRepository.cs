using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories.Interfaces;

namespace PaymentGateway.Api.Repositories;

public class PaymentsRepository :IPaymentsRepository
{
    public List<Payments> Payments = [];
    
    public void Add(Payments payment)
    {
        Payments.Add(payment);
    }

    public Payments? Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}