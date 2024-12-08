using ExampleApp.Clients;
using ExampleApp.ExampleCreator;
using ExampleApp.ExampleCreator.v2;
using ExampleApp.Services;
using Shared;

var currentClient =new ClientEnvironmentModel()
{
    Id= "b*********f66",
    Secret= "S*****mUmY="
};
var client = new CoinPaymentsApiClient("https://api.coinpayments.com/api/v1", currentClient);

var invoiceExampleCreatorV2 = new InvoiceV2ExampleCreator(client);
await invoiceExampleCreatorV2.CreateInvoiceInUSD(10.5M);
