using ExampleApp.Clients;
using ExampleApp.ExampleCreator;
using ExampleApp.Services;
using Shared;

var currentClient =new ClientEnvironmentModel()
{
    Id= "bb1194edff814fabafb567c12e43df66",
    Secret= "SCrYiYalS6XvksCfidIOO1YgAnFu+ht1MrxVnSHmUmY="
};
var client = new CoinPaymentsApiClient("https://api.coinpayments.com/api/v1", currentClient);
var exampleCreator = new ExampleCreator(client);
await exampleCreator.CreateInvoiceInUSD();
