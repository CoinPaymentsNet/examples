using ExampleApp.Clients;
using ExampleApp.ExampleCreator;
using ExampleApp.Services;
using Shared;

var currentClient =new ClientEnvironmentModel()
{
    Id= "",
    Secret= ""
};
var client = new CoinPaymentsApiClient("https://api.coinpayments.net/api/v1", currentClient);
var exampleCreator = new ExampleCreator(client);
await exampleCreator.CreateInvoiceInUSD();
