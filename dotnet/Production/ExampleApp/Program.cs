using ExampleApp.Clients;
using ExampleApp.Services;
using Shared;
using Shared.Models;


var prodClient = new ProdApiClient("https://api.coinpayments.com/api/v1");
var invoiceService = new InvoiceService(prodClient);

// await invoiceService.CreateInvoiceInUSD_and_List_LTCT_Payouts(20_00);//Must be specified in cents 20_00=> $20

var rateService = new RateService(prodClient);
var usdAmount = 30_00;//Must be specified in cents 20_00=> $20
var amount = await rateService.GetUSDRateSmallestUnitByCurrencyId(Currencies.BTC.Id.ToString(), usdAmount);
Console.WriteLine($" {Currencies.CoinValueFromSmallestUnit(Currencies.USD.Id, usdAmount)} is {Currencies.CoinValueFromSmallestUnit(Currencies.BTC.Id, amount)}");