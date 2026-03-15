using System.Net;

using Microsoft.Extensions.Logging;

using Moq;

using PaymentGateway.Api.External.Interfaces;
using PaymentGateway.Api.External.Models.Request;
using PaymentGateway.Api.External.Models.Response;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Repositories.Interfaces;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Services.Interfaces;

namespace PaymentGateway.Api.UnitTests
{
    public class PaymentServiceTests
    {
        private readonly Mock<IBankHttpClient> _bankHttpClientMock = new();
        private readonly Mock<IPaymentsRepository> _paymentrepositoryMock = new();
        private readonly Mock<ILogger<IPaymentsService>> _loggerMock = new();

        [Fact]
        public void GetPayment_ReturnsPayment_WhenExists()
        {
            var id = Guid.NewGuid();

            var payment = new Payments
            {
                Id = id,
                CardNumberLastFour = "1234",
                ExpiryMonth = 12,
                ExpiryYear = 2030,
                Currency = "GBP",
                Amount = 100,
                Status = (int)PaymentStatus.Authorized
            };

            _paymentrepositoryMock.Setup(r => r.Get(id)).Returns(payment);

            var service = new PaymentsService(
                _bankHttpClientMock.Object,
                _paymentrepositoryMock.Object,
                _loggerMock.Object);

            var result = service.GetPayment(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("1234", result.CardNumberLastFour);
        }

        [Fact]
        public async Task ProcessPaymentAsync_AuthorizePayment()
        {
            var request = new PostPaymentRequest
            {
                CardNumber = "1234567812345679",
                ExpiryMonth = 12,
                ExpiryYear = 2028,
                Currency = "GBP",
                Amount = 100,
                CVV = "123"
            };

            var bankResponse = new ApiResponse<BankPaymentResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Data = new BankPaymentResponse { Authorized = true, AuthorizationCode = "s233455-1243455-23435" }
            };

            _bankHttpClientMock
                .Setup(b => b.PostAsync<BankPaymentRequest, BankPaymentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<BankPaymentRequest>()))
                .ReturnsAsync(bankResponse);

            var service = new PaymentsService(
                _bankHttpClientMock.Object,
                _paymentrepositoryMock.Object,
                _loggerMock.Object);

            var result = await service.ProcessPaymentAsync(request);

            Assert.NotNull(result);
            Assert.Equal(PaymentStatus.Authorized, result.Status);
        }

        [Fact]
        public async Task ProcessPaymentAsync_DeclinePayment()
        {
            var request = new PostPaymentRequest
            {
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                ExpiryYear = 2028,
                Currency = "GBP",
                Amount = 100,
                CVV = "123"
            };

            var bankResponse = new ApiResponse<BankPaymentResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Data = new BankPaymentResponse { Authorized = false}
            };

            _bankHttpClientMock
                .Setup(b => b.PostAsync<BankPaymentRequest, BankPaymentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<BankPaymentRequest>()))
                .ReturnsAsync(bankResponse);

            var service = new PaymentsService(
                _bankHttpClientMock.Object,
                _paymentrepositoryMock.Object,
                _loggerMock.Object);

            var result = await service.ProcessPaymentAsync(request);

            Assert.NotNull(result);
            Assert.Equal(PaymentStatus.Declined, result.Status);
        }


        [Fact]
        public async Task ProcessPaymentAsync_RejectedPayment()
        {
            var request = new PostPaymentRequest
            {
                CardNumber = "1234567812345670",
                ExpiryMonth = 12,
                ExpiryYear = 2028,
                Currency = "GBP",
                Amount = 100,
                CVV = "123"
            };

            var bankResponse = new ApiResponse<BankPaymentResponse>
            {
                StatusCode = HttpStatusCode.ServiceUnavailable
            };

            _bankHttpClientMock
                .Setup(b => b.PostAsync<BankPaymentRequest, BankPaymentResponse>(
                    It.IsAny<string>(),
                    It.IsAny<BankPaymentRequest>()))
                .ReturnsAsync(bankResponse);

            var service = new PaymentsService(
                _bankHttpClientMock.Object,
                _paymentrepositoryMock.Object,
                _loggerMock.Object);

            var result = await service.ProcessPaymentAsync(request);

            Assert.NotNull(result);
            Assert.Equal(PaymentStatus.Rejected, result.Status);
        }
    }
}