using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Services.Interfaces;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentsService paymentsService, ILogger<PaymentsController> logger) : Controller
{
    /// <summary>
    /// Returns Payment for payment id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public ActionResult<PostPaymentResponse?> GetPayment(Guid id)
    {
        try
        {
            var payment = paymentsService.GetPayment(id);
            return payment == null ? (ActionResult<PostPaymentResponse?>)new NotFoundResult() : (ActionResult<PostPaymentResponse?>)new OkObjectResult(payment);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving payment with ID {PaymentId}", id);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Adds Payment
    /// </summary>
    /// <param name="paymentRequest"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse?>> PostPaymentAsync(PostPaymentRequest paymentRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await paymentsService.ProcessPaymentAsync(paymentRequest);
            return result == null ? (ActionResult<PostPaymentResponse?>)new NotFoundResult() : (ActionResult<PostPaymentResponse?>)new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing payment request");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}