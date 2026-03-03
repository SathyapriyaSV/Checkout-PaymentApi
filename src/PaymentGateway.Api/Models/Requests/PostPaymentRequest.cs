using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest
{
    [Required]
    [StringLength(19, MinimumLength = 14)]
    [RegularExpression(@"^\d+$", ErrorMessage = "Card number must contain only numeric characters.")]
    public required string CardNumber { get; set; }

    [Required]
    [Range(1, 12, ErrorMessage = "Expiry month must be between 1 and 12.")]
    public int ExpiryMonth { get; set; }

    [Required]
    public int ExpiryYear { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public required string Currency { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Amount must be a positive integer.")]
    public int Amount { get; set; } // Minor currency unit

    [Required]
    [StringLength(4, MinimumLength = 3)]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3-4 numeric characters.")]
    public required string CVV { get; set; }

    // Limit validation to no more than 3 ISO currency codes
    private static readonly List<string> AllowedCurrencies = ["USD", "EUR", "GBP"];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // Validate currency against allowed ISO codes (max 3 as required)
        if (!string.IsNullOrWhiteSpace(Currency) &&
            !AllowedCurrencies.Contains(Currency.ToUpperInvariant()))
        {
            results.Add(new ValidationResult(
                $"Currency must be one of the following: {string.Join(", ", AllowedCurrencies)}.",
                [nameof(Currency)]));
        }

        // Validate expiry year is in the future
        var now = DateTime.UtcNow;
        var currentYear = now.Year;
        var currentMonth = now.Month;

        if (ExpiryYear < currentYear)
        {
            results.Add(new ValidationResult(
                "Expiry year must be in the future.",
                [nameof(ExpiryYear)]));
        }
        else if (ExpiryYear == currentYear && ExpiryMonth < currentMonth)
        {
            results.Add(new ValidationResult(
                "Expiry month and year combination must be in the future.",
                [nameof(ExpiryMonth), nameof(ExpiryYear)]));
        }

        return results;
    }
}