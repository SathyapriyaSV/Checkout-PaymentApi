using PaymentGateway.Api.Clients;
using PaymentGateway.Api.External.Interfaces;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Repositories.Interfaces;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddScoped<IPaymentsService, PaymentsService>();

//BankHttpClient
builder.Services.AddHttpClient<IBankHttpClient, BankHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BankApi:BaseUrl"]!);

    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
