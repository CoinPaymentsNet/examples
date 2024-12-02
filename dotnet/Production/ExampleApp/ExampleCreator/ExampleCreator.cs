using ExampleApp.Clients;
using Shared;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleApp.ExampleCreator
{
    public class ExampleCreator(CoinPaymentsApiClient _client)
    {/// <summary>
     /// To create an invoice in USD for the specified amount
     /// </summary>
     /// <param name="amountAsUsd">The amount of money must be specified in cents</param>
     /// <returns></returns>
        public async Task CreateInvoiceInUSD(int amountAsUsd = 10_00)
        {
            var simpleRequest = new
            {
                ClientId = _client.CurrentClient.Id,
                Items = new object[]{
                    new {
                        Name="test item",
                        Quantity=new{Value=1,Type=2},
                        OriginalAmount= new {CurrencyId="5057", Value= amountAsUsd }, //5057 is USD
                        Amount= new { CurrencyId="5057", Value= amountAsUsd },
                        }
                },
                Amount = new
                {
                    CurrencyId = "5057",
                    Value = amountAsUsd,
                    Breakdown = new
                    {
                        Subtotal = new { CurrencyId = "5057", Value = amountAsUsd },
                        Shipping = new { CurrencyId = "5057", Value = 0 },
                        Handling = new { CurrencyId = "5057", Value = 0 },
                        TaxTotal = new { CurrencyId = "5057", Value = 0 },
                        Discount = new { CurrencyId = "5057", Value = 0 },
                    }
                },
                MerchantOptions = new
                {
                    ShowEmail = true,
                }
            };
            var newInvoice =
                await _client.AuthExecuteAsync<CreateMerchantInvoiceResponse>("merchant/invoices",
                    HttpMethod.Post, _client.CurrentClient.Id, _client.CurrentClient.Secret, simpleRequest);

            var invoice = newInvoice.Invoices.First();
            var invoiceId = Guid.Parse(invoice.Id);

            var request = new { RefundEmail = $"{Random.Shared.Next()}@in.crypto" };
            var payment = await _client.AuthExecuteAsync<InvoicePaymentDto>(
                $"invoices/{invoiceId}/payments", HttpMethod.Post, _client.CurrentClient.Id,
                _client.CurrentClient.Secret, request);
            
            var targetCurrencyId = Currencies.LTCT.GetId();
            var paymentDetails = await _client.AuthExecuteAsync<InvoicePaymentCurrencyPaymentDetailsDto>(
                $"invoices/{invoiceId}/payment-currencies/{targetCurrencyId}", HttpMethod.Get,
                _client.CurrentClient.Id, _client.CurrentClient.Secret, null);

            Console.WriteLine($"Awaiting {payment.PaymentCurrencies.FirstOrDefault(x => x.Currency.Id == targetCurrencyId).RemainingAmount.DisplayValue} " +
                $"LTCT amount on {paymentDetails?.Addresses.Address}");

            var payoutSetting = await _client.AuthExecuteAsync<InvoicePayoutsDetailsDto>(
                $"merchant/invoices/{invoiceId}/payouts", HttpMethod.Get, _client.CurrentClient.Id,
                _client.CurrentClient.Secret, null);

            var invoiceStatusResponse = await _client.AuthExecuteAsync<InvoiceStatusDtoResponseDto>($"invoices/{invoiceId}/payment-currencies/{targetCurrencyId}/status", HttpMethod.Get, _client.CurrentClient.Id,
                _client.CurrentClient.Secret, null);
        }
    }
}
