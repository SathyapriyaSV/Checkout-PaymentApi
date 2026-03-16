using System.Collections.Concurrent;

using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories.Interfaces;

namespace PaymentGateway.Api.Repositories;

public class PaymentsRepository :IPaymentsRepository
{
    public ConcurrentDictionary<Guid, Payments> Payments = [];
    
    public void Add(Payments payment)   
    {
        Payments[payment.Id] = payment;
    }

    public Payments? Get(Guid id)
    {
        Payments.TryGetValue(id, out var payment);
        return payment;
    }
}