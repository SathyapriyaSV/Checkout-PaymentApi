using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services.Interfaces;

namespace PaymentGateway.Api.UnitTests
{
    public class PaymentControllerTests
    {
        private readonly Mock<IPaymentsService> _paymentsServiceMock;
        private readonly Mock<ILogger<PaymentsController>> _loggerMock;
        private readonly PaymentsController _controller;
        public PaymentControllerTests()
        {
            _paymentsServiceMock = new Mock<IPaymentsService>();
            _loggerMock = new Mock<ILogger<PaymentsController>>();

            _controller = new PaymentsController(
                _paymentsServiceMock.Object,
                _loggerMock.Object);
        }
        [Fact]
        public async Task GetPayment_ReturnsOk_WhenPaymentExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var paymentResponse = new PostPaymentResponse() { Id = id};

            _paymentsServiceMock
                .Setup(x => x.GetPayment(id))
                .Returns(paymentResponse);

            // Act
            var result = _controller.GetPayment(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(paymentResponse, okResult.Value);
        }

        [Fact]
        public async Task GetPayment_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();

            _paymentsServiceMock
                .Setup(x => x.GetPayment(id))
                .Returns((PostPaymentResponse?)null);

            // Act
            var result = _controller.GetPayment(id);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetPayment_Returns500_WhenExceptionOccurs()
        {
            // Arrange
            var id = Guid.NewGuid();

            _paymentsServiceMock
                .Setup(x => x.GetPayment(id))
                .Throws(new Exception());

            // Act
            var result = _controller.GetPayment(id);

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task PostPaymentAsync_ReturnsBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("error", "invalid");

            var request = new PostPaymentRequest();

            // Act
            var result = await _controller.PostPaymentAsync(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostPaymentAsync_ReturnsOk_WhenPaymentProcessed()
        {
            // Arrange
            var request = new PostPaymentRequest();
            var response = new PostPaymentResponse();

            _paymentsServiceMock
                .Setup(x => x.ProcessPaymentAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.PostPaymentAsync(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task PostPaymentAsync_ReturnsNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            var request = new PostPaymentRequest();

            _paymentsServiceMock
                .Setup(x => x.ProcessPaymentAsync(request))
                .ReturnsAsync((PostPaymentResponse?)null);

            // Act
            var result = await _controller.PostPaymentAsync(request);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostPaymentAsync_Returns500_WhenExceptionOccurs()
        {
            // Arrange
            var request = new PostPaymentRequest();

            _paymentsServiceMock
                .Setup(x => x.ProcessPaymentAsync(request))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.PostPaymentAsync(request);

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}
