using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentsService paymentsService, ILogger<PaymentsController> logger) : Controller
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = paymentsService.GetPayment(id);
        return payment == null ? (ActionResult<PostPaymentResponse?>)new NotFoundResult() : (ActionResult<PostPaymentResponse?>)new OkObjectResult(payment);
    }
    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse?>> PostPaymentAsync(PostPaymentRequest paymentRequest)
    {
        var payment = paymentsService.ProcessPaymentAsync(paymentRequest); 
        return payment == null ? (ActionResult<PostPaymentResponse?>)new NotFoundResult() : (ActionResult<PostPaymentResponse?>)new OkObjectResult(payment);
    }
}