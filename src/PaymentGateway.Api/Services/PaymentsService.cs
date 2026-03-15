using PaymentGateway.Api.External.Interfaces;
using PaymentGateway.Api.External.Models.Request;
using PaymentGateway.Api.External.Models.Response;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories.Interfaces;
using PaymentGateway.Api.Services.Interfaces;

namespace PaymentGateway.Api.Services
{
    public class PaymentsService(IBankHttpClient bankHttpClient, IPaymentsRepository repository, ILogger<IPaymentsService> logger) : IPaymentsService
    {
        private readonly IBankHttpClient _bankHttpClient = bankHttpClient;
        private readonly IPaymentsRepository _repository = repository;

        //Leaving it Synchronous as it is in-memory and not doing any I/O operations, but can be made asynchronous if needed in the future when integrating with external services.
        public PostPaymentResponse? GetPayment(Guid id)
        {
            var result = _repository.Get(id);

            return result != null
                ? MapToPostPaymentResponse(result)
                : null;
        }

        public async Task<PostPaymentResponse?> ProcessPaymentAsync(PostPaymentRequest request)
        {
            try
            {
                BankPaymentRequest paymentRequest = new()
                {
                    CardNumber = request.CardNumber!,
                    ExpiryDate = string.Concat(request.ExpiryMonth, "/", request.ExpiryYear),
                    Currency = request.Currency!,
                    Amount = request.Amount,
                    Cvv = request.CVV!
                };
                //Call bank API
                var paymentResponse = await _bankHttpClient.PostAsync<BankPaymentRequest, BankPaymentResponse>("payments", paymentRequest);

                //Add to in-memory Repository if response is not null
                if (paymentResponse != null)
                {
                    Payments payment = new()
                    {
                        Id = Guid.NewGuid(),
                        CardNumberLastFour = request.CardNumber![^4..],
                        ExpiryMonth = request.ExpiryMonth,
                        ExpiryYear = request.ExpiryYear,
                        Currency = request.Currency,
                        Amount = request.Amount,
                        Status = paymentResponse switch
                        {
                            { IsSuccess: false } => (int) PaymentStatus.Rejected,
                            { Data.Authorized: true } => (int) PaymentStatus.Authorized,
                            _ => (int) PaymentStatus.Declined
                        }
                    };

                    _repository.Add(payment);
                    return MapToPostPaymentResponse(payment);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Payment processing failed for card ending {request.CardNumber![^4..]} ");
                throw;
            }

            return null;
        }

        private static PostPaymentResponse MapToPostPaymentResponse(Payments payments)
        {
            return new()
            {
                Id = payments.Id,
                Amount = payments.Amount,
                CardNumberLastFour = payments.CardNumberLastFour,
                Currency = payments.Currency,
                ExpiryMonth = payments.ExpiryMonth,
                ExpiryYear = payments.ExpiryYear,
                Status = (PaymentStatus) payments.Status
            };

        }
    }

}
