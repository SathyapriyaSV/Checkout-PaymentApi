using System.Net;

namespace PaymentGateway.Api.External.Models.Response
{
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; init; }
        public T? Data { get; init; }
        public string? ErrorContent { get; init; }
        public bool IsSuccess => ((int)StatusCode >= 200 && (int)StatusCode < 300);

    }
}
