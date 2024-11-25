using InvoiceApp.Clients;
using InvoiceApp.Services;
using Shared;

var currentClient =new ClientEnvironmentModel()
{
    Id= "bb****43df66",
    Secret= "SC****="
};
var prodClient = new ProdApiClient("https://api.coinpayments.com/api/v1", currentClient);
var invoiceService = new InvoiceService(prodClient);

await invoiceService.CreateInvoiceInUSD_and_List_LTCT_Payouts(20_00);//Must be specified in cents 20_00=> $20
