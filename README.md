# Payment Gateway API

This is the .NET version of the Payment Gateway challenge, implements the APIs listed under [README.md](https://github.com/cko-recruitment/). 

## Template structure
```
src/
    PaymentGateway.Api - Simple ASP.NET Core Web API for Payment Gateway Challenge
test/
    PaymentGateway.Api.Tests - an empty xUnit test project
    PaymentGateway.Api.UnitTests - an empty xUnit Unit test project
imposters/ - contains the bank simulator configuration. Don't change this

.editorconfig - don't change this. It ensures a consistent set of rules for submissions when reformatting code
docker-compose.yml - configures the bank simulator
PaymentGateway.sln
```
## To compile and run
# Run Bank API
    docker-compose up
# Clone and run
    git clone https://github.com/SathyapriyaSV/Checkout-PaymentApi.git
    cd Checkout-PaymentApi/src/PaymentGateway.Api
    dotnet restore
    dotnet build
    dotnet run