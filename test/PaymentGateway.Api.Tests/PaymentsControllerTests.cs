using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using PaymentGateway.Api.External.Interfaces;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Repositories.Interfaces;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new Payments
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999).ToString(),
            Currency = "GBP",
            Status = _random.Next(0, 3)
        };

        var paymentsRepository = new PaymentsRepository();
        paymentsRepository.Add(payment);

        var webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IPaymentsRepository>(paymentsRepository); 
                });

            });

        var client = webApplicationFactory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }
    [Fact]
    public async Task AddsAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentRequest
        {
            ExpiryYear = _random.Next(2027, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumber = string.Concat(Enumerable.Range(0, 16)
                    .Select(_ => _random.Next(0, 10))),
            Currency = "GBP",
            CVV = _random.Next(111, 999).ToString()

        };

        var paymentsRepository = new PaymentsRepository();

        var webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IBankHttpClient>();
                    services.AddSingleton<IBankHttpClient, FakeBankHttpClient>();
                });
            });

        var client = webApplicationFactory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync($"/api/Payments/", payment);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }
    [Fact]
    public async Task CanCreateAndRetrievePayment()
    {
        // Arrange
        var payment = new PostPaymentRequest
        {
            ExpiryYear = _random.Next(2027, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumber = string.Concat(Enumerable.Range(0, 16)
                    .Select(_ => _random.Next(0, 10))),
            Currency = "GBP",
            CVV = _random.Next(111, 999).ToString()
        };
        var paymentsRepository = new PaymentsRepository();
        var webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IBankHttpClient>();
                services.AddSingleton<IBankHttpClient, FakeBankHttpClient>();
            });
        });


        var client = webApplicationFactory.CreateClient();

        //Act
        var postResponse = await client.PostAsJsonAsync("/api/payments", payment);
        var created = await postResponse.Content.ReadFromJsonAsync<PostPaymentResponse>(JsonOptions);

        var getResponse = await client.GetAsync($"/api/payments/{created?.Id}");
        var retrieved = await getResponse.Content.ReadFromJsonAsync<PostPaymentResponse>(JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        Assert.NotNull(retrieved);
        Assert.NotNull(created);
        Assert.Equal(created!.Id, retrieved!.Id);
        Assert.Equal(created.Amount, retrieved.Amount);
        Assert.Equal(created.Currency, retrieved.Currency);
        Assert.Equal(retrieved.Id, created.Id);
    }
    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<Program>();
        var client = webApplicationFactory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

}