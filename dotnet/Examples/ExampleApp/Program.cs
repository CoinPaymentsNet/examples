using ExampleApp.Clients;
using ExampleApp.ExampleCreator;
using ExampleApp.ExampleCreator.V2;
using Shared;
using Shared.Models;

var client = new CoinPaymentsApiPublicClient("https://api.coinpayments.net/api/v2");
var currency = Currencies.BTC;
var feeCreator = new BlockChainFeeCreator(client);
var btcBlockChainFee = await feeCreator.GetBlockchainFeeByGivenCurrency(currency.GetId());
Console.WriteLine($"Blockchain fee for {currency.Name} => {btcBlockChainFee}");