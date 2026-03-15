using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.External.Models.Request
{
    public class BankPaymentRequest
    {
        [JsonPropertyName("card_number")]
        public required string CardNumber { get; set; } 

        [JsonPropertyName("expiry_date")]
        public required string ExpiryDate { get; set; }  // MM/yyyy

        [StringLength(3, MinimumLength = 3)]
        [JsonPropertyName("currency")]
        public required string Currency { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("cvv")]
        public required string Cvv { get; set; } 
    }
}
