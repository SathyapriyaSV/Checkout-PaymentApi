using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.UnitTests
{
    public class PostPaymentRequestValidationTests
    {
        [Fact]
        public void PostPaymentRequest_ShouldFail_WhenCurrencyIsNotAllowed()
        {
            var request = new PostPaymentRequest
            {
                CardNumber = "1234567890123456",
                ExpiryMonth = 12,
                ExpiryYear = DateTime.UtcNow.Year + 1,
                Currency = "INR",
                Amount = 100,
                CVV = "123"
            };

            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("Currency"));
        }

        [Fact]
        public void PostPaymentRequest_ShouldFail_WhenExpiryDateInPast()
        {
            var request = new PostPaymentRequest
            {
                CardNumber = "1234567890123456",
                ExpiryMonth = 12,
                ExpiryYear = DateTime.UtcNow.Year - 1,
                Currency = "GBP",
                Amount = 100,
                CVV = "123"
            };

            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("ExpiryYear"));
        }
    }
}
