using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Clients.Models
{
    public class PaymentResponse
    {
        [JsonPropertyName("authorized")]
        public bool Authorized { get; set; } 

        [JsonPropertyName("authorization_code")]
        public Guid AuthorizationCode { get; set; } 
    }
}
