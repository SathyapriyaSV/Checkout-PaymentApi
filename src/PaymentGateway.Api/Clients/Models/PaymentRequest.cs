using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Clients.Models
{
    public class PaymentRequest
    {
        [JsonPropertyName("card_number")]
        public string CardNumber { get; set; } 

        [JsonPropertyName("expiry_date")]
        public string ExpiryDate { get; set; }  // MM/yyyy

        [StringLength(3, MinimumLength = 3)]
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("cvv")]
        public string Cvv { get; set; } 
    }
}
