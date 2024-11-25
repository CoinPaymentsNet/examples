using ExampleApp.Clients;
using Shared.Models;
using System.Linq.Expressions;

namespace ExampleApp.Services
{
    public class InvoiceService(ProdApiClient _prodClient)
    {
        private MerchantWallet[] _prodMerchantWallets;

        private async Task GetProdMerchantWallets()
        {
            _prodMerchantWallets = await _prodClient.GetMerchantWallets();
        }
        public async Task CreateInvoiceInUSD_and_List_LTCT_Payouts(int amountAsUsd = 10_00)
        {

            var newInvoice = await _prodClient.CreateInvoice(amount: amountAsUsd);
            var invoice = newInvoice.Invoices.First();
            var invoiceId = Guid.Parse(invoice.Id);
            var payment = await _prodClient.CreateInvoicePayments(invoiceId.ToString());
            var paymentDetails =
                await _prodClient.GetInvoicePaymentCurrencyDetails(invoiceId.ToString(), Currencies.LTCT.Id);
            Console.WriteLine($"Awaiting {payment.PaymentCurrencies.FirstOrDefault(x => x.Currency.Id == Currencies.LTCT.Id.ToString()).RemainingAmount.DisplayValue} " +
                $"LTCT amount on {paymentDetails?.Addresses.Address}");
            var payoutSetting = await _prodClient.GetPayoutDetails(invoiceId);

            var invoiceStatusResponse = await _prodClient.GetInvoicePaymentStatus(invoice.Id, Currencies.LTCT.Id, null);
            ///The following lines can be used to pay the invoice related to another wallet of the current merchant..
           /*
            //await GetProdMerchantWallets();

            //var highestBalanceWallet = _prodMerchantWallets.Where(x => x.CurrencyId == Currencies.LTCT.Id)
            //    .OrderByDescending(x => Convert.ToInt64(x.ConfirmedBalance?.Value)).FirstOrDefault();

            var spendRequest = await _prodClient.RequestSpendFromMerchantWallet(Currencies.LTCT.Id,
                Guid.Parse(highestBalanceWallet.WalletId), paymentDetails.Addresses.Address,
                Convert.ToDecimal(paymentDetails.Amount.Value));

            var allFees = spendRequest.BlockchainFee + spendRequest.CoinpaymentsFee;
            await _prodClient.ConfirmSpendFromMerchantWallet(Guid.Parse(highestBalanceWallet.WalletId),
                spendRequest.SpendRequestId);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var isInvoiceStatusPaid = false;
            while (sw.Elapsed <= TimeSpan.FromMinutes(3))
            {
                await Task.Delay(2000);
                invoiceStatusResponse = await _prodClient.GetInvoicePaymentStatus(invoice.Id, Currencies.LTCT.Id, null);
                if (invoiceStatusResponse.Status == InvoiceStatus.Paid)
                {
                    isInvoiceStatusPaid = true;
                    sw.Stop();
                    break;
                }
            }

            if (!isInvoiceStatusPaid)
            {
                throw new Exception("Invoice payment failed cause to invoice not paid in 3 minutes.");
            }


            await Task.Delay(5000);
            payoutSetting = await _prodClient.GetPayoutDetails(invoiceId);
            var allMerchantFees = payoutSetting.Items.Sum(x => x.MerchantFees.TransactionFee.ValueAsDecimal) +
                                  //payoutSetting.Items.Sum(x => x.MerchantFees.ConversionFee.ValueAsDecimal) +
                                  payoutSetting.Items.Sum(x => x.MerchantFees.NetworkFee.ValueAsDecimal);

            var beforeSourceBalance = Convert.ToDecimal(highestBalanceWallet.ConfirmedBalance.Value);
            var actualSourceBalance = Convert.ToDecimal(highestBalanceWallet.ConfirmedBalance.Value) - allFees -
                                      Convert.ToDecimal(paymentDetails.Amount.Value);

            await GetProdMerchantWallets();

            highestBalanceWallet = GetProdMerchantWalletBy(x => x.WalletId == highestBalanceWallet.WalletId);

            // GetProdMerchantWalletBy(x => x.DepositAddress == paymentDetails.Addresses.Address);
            if (Convert.ToDecimal(highestBalanceWallet.ConfirmedBalance.Value) == actualSourceBalance)
            {

            }
            else
            {
                throw new Exception("Invoice payment failed. Source and target wallet's amount are not valid");
            }
           */
        }
        MerchantWallet? GetProdMerchantWalletBy(Expression<Func<MerchantWallet, bool>> exp) =>
                _prodMerchantWallets.FirstOrDefault(exp.Compile());


    }

}
