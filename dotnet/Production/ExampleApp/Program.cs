using ExampleApp.Clients;
using ExampleApp.ExampleCreator;
using ExampleApp.Services;
using Shared;
using Shared.Models;

var currentClient =new ClientEnvironmentModel()
{
    Id= "bb1194edff814fabafb567c12e43df66",
    Secret= "SCrYiYalS6XvksCfidIOO1YgAnFu+ht1MrxVnSHmUmY="
};
var client = new CoinPaymentsApiClient("https://api.coinpayments.com/api/v1", currentClient);


// await invoiceService.CreateInvoiceInUSD_and_List_LTCT_Payouts(20_00);//Must be specified in cents 20_00=> $20

var rateService = new RateService(client);
var usdAmount = 30_00;//Must be specified in cents 20_00=> $20
var amount = await rateService.GetUSDRateSmallestUnitByCurrencyId(Currencies.BTC.Id.ToString(), usdAmount);
Console.WriteLine($" {Currencies.CoinValueFromSmallestUnit(Currencies.USD.Id, usdAmount)} is {Currencies.CoinValueFromSmallestUnit(Currencies.BTC.Id, amount)}");