using ExampleApp.Services;
using Shared;
using Shared.Models;
using System.Text.Json;

var cbService = new CallbackService();

var clientId = "bb11****e43df66";
var secret = "SC*******HmUmY=";
var expectedSignature = "yuS+GsMZO8KYhX6QCXvS5Mx6ume3h09ZezUCU5BCq9s=";
var timestampHeader = "2024-09-10T11:08:37";
var requestUrl = "https://paymentcallback5.upgaming.online/api/CoinPaymentsV2/Callback";
var notificationBody = new NotificationDto(
    Guid.Parse("7d5d7327-b21b-4cd4-9883-9d3a996842b7"),
    "3FVwhH9U2HLWckSaoT8VEfchxVWosHiNba",
    Guid.Parse("5f26617d-379c-4966-9a90-3e92d6a1dd1b"),
    "aeb6d8a0687e7290e65e21140b56958ce3a298bf99c7d60b64a815b66341970c",
    Guid.Parse("5c7f68e0-77cc-4d9c-a334-5fb56dd26efc"),
    WalletTransactionType.UtxoExternalReceive,
    "0.00017412",
    "BTC",
    "0.00000087",
    "BTC",
    "0.00000062",
    "BTC",
    "0.00017561",
    "BTC",
    "16.00",
    "0.1",
    "0.2",
    "16.3",
    "USD",
    2,
    2);
var isEqual = cbService.ValidateSignatureAndCreateCallback(clientId, secret, requestUrl, expectedSignature, timestampHeader, notificationBody);
if (isEqual && CallbackService.Notifications.TryPop(out var currentNotification))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Signature valid, notification created\n");
    Console.WriteLine(JsonSerializer.Deserialize<NotificationDto>(currentNotification));
    Console.ForegroundColor = ConsoleColor.White;
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Signature invalid, please check credentials");
    Console.ForegroundColor = ConsoleColor.White;
}

