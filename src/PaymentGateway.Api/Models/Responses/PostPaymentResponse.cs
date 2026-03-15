namespace PaymentGateway.Api.Models.Responses;

public class PostPaymentResponse
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public string? CardNumberLastFour { get; set; } // Masked card number (last 4 digits only). Stored as string to preserve leading zeros.
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string? Currency { get; set; }
    public int Amount { get; set; }
}
