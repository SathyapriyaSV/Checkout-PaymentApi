using PaymentGateway.Api.Clients.Models;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services
{
    public class PaymentsService(IBankHttpClient bankHttpClient, IPaymentsRepository repository, ILogger<PaymentsService> logger) : IPaymentsService
    {
        private readonly IBankHttpClient _bankHttpClient = bankHttpClient;
        private readonly IPaymentsRepository _repository = repository;

        //Leaving it Synchronous as it is in-memory and not doing any I/O operations, but can be made asynchronous if needed in the future when integrating with external services.
        public PostPaymentResponse? GetPayment(Guid id) 
        {
            return _repository.Get(id);
        }

        public async Task<PostPaymentResponse?> ProcessPaymentAsync(PostPaymentRequest request) 
        {
            PostPaymentResponse? response = null;
            PaymentRequest paymentRequest = new PaymentRequest
            {
                CardNumber = request.CardNumber,
                ExpiryDate = string.Concat(request.ExpiryMonth, "/", request.ExpiryYear),
                Currency = request.Currency,
                Amount = request.Amount,
                Cvv = request.CVV
            };
            //Call bank API
            var paymentResponse = await _bankHttpClient.PostAsync<PaymentRequest, ApiResponse<PaymentResponse>>("payments", paymentRequest);

            //Add to in-memory Repository if response is not null
            if (paymentResponse != null)
            {
                PostPaymentResponse postPaymentResponse = new PostPaymentResponse()
                {
                    Id = Guid.NewGuid(),
                    CardNumberLastFour = request.CardNumber.Substring(request.CardNumber.Length - 4),
                    ExpiryMonth = request.ExpiryMonth,
                    ExpiryYear = request.ExpiryYear,
                    Currency = request.Currency,
                    Amount = request.Amount
                };

               if (paymentResponse.IsSuccess)
                {
                    if (paymentResponse.Data?.Data?.Authorized == true)
                    {
                        postPaymentResponse.Status = PaymentStatus.Authorized;
                    }
                    else
                    {
                        postPaymentResponse.Status = PaymentStatus.Declined;
                    }
                }
                else
                {
                    postPaymentResponse.Status = PaymentStatus.Rejected;
                }
                _repository.Add(postPaymentResponse);
            }
            return response;
        }
    }

}
