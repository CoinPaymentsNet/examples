using ExampleApp.Clients;
using ExampleApp.ExampleCreator;
using ExampleApp.Services;
using Shared;
using Shared.Models;

var client = new CoinPaymentsApiClient("https://api.coinpayments.net/api/v1");

var rateService = new RateService(client);
var usdAmount = 30_00;//Must be specified in cents 20_00=> $20
var amount = await rateService.GetUSDRateSmallestUnitByCurrencyId(Currencies.BTC.Id.ToString(), usdAmount);
Console.WriteLine($" {Currencies.CoinValueFromSmallestUnit(Currencies.USD.Id, usdAmount)} is {Currencies.CoinValueFromSmallestUnit(Currencies.BTC.Id, amount)}");