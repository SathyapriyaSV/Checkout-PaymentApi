using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.External.Models.Response
{
    public class BankPaymentResponse
    {
        [JsonPropertyName("authorized")]
        public bool Authorized { get; set; } 

        [JsonPropertyName("authorization_code")]
        public string? AuthorizationCode { get; set; } 
    }
}
