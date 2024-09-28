using ConsolidationApp.Models.Currencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolidationApp.Models.Invoice
{
    public class InvoicePaymentHotWalletModel
    {
        public Guid Id { get; init; }
        public Currency Currency { get; init; }

        //public required Guid PooledWalletId { get; set; }

        public Guid? MerchantPayoutWalletId { get; init; }

        public required InvoiceCurrencyPaymentFeesModel BuyerFees { get; init; } = null!;

        public required InvoiceCurrencyPaymentFeesModel MerchantFees { get; init; } = null!;

        public decimal MerchantMarkupOrDiscount { get; init; }

        public decimal PaymentSubTotal { get; init; }

        public string? MerchantPayoutAddress { get; init; }

        public Currency MerchantPayoutCurrency { get; init; }

        public decimal CurrencyRateFromInvoiceCurrency { get; init; }

        public string? PaymentReceiveAddress { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

        public bool IsConversion { get; init; }

        public EstimatingPaymentErrorModel? Error { get; init; }

        public int PayoutFrequency { get; init; }

        public Guid InvoicePaymentId { get; init; }

        public decimal TotalBuyerWillPay => PaymentSubTotal + BuyerFees.Total + MerchantMarkupOrDiscount;

        public decimal TotalMerchantWillReceive => PaymentSubTotal + MerchantMarkupOrDiscount - MerchantFees.Total;

        public decimal TotalCoinPaymentsServiceFees => BuyerFees.ConversionFee + MerchantFees.CoinPaymentsServiceFee;

        public decimal TotalNetworkFees => BuyerFees.NetworkFee + MerchantFees.NetworkFee + MerchantFees.ConversionFee;

        public int Confirmations { get; init; }

        public decimal ConfirmedAmount { get; init; }

        public int RequiredConfirmations { get; init; }

        public decimal UnconfirmedAmount { get; init; }

        public InvoicePaymentHotWalletAssignmentModel? Assignment { get; init; }

        public Guid? PooledWalletId { get; init; }

        public DateTimeOffset ExpiresAt { get; set; }
    }
    public record class InvoiceCurrencyPaymentFeesModel(
        decimal CoinPaymentsServiceFee,
        decimal NetworkFee,
        decimal MarkupOrDiscount,
        decimal ConversionFee)
    {
        /// <summary>
        /// the total fees in the payment currencies smallest units (e.g. Satoshis for BTC)
        /// </summary>
        public decimal Total => CoinPaymentsServiceFee + NetworkFee + ConversionFee + MarkupOrDiscount;
    }
    public class EstimatingPaymentErrorModel
    {
        public EstimatingPaymentErrorModel(InvoicePaymentSettlementModeErrorModel code, string message)
        {
            Code = code;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public InvoicePaymentSettlementModeErrorModel Code { get; }

        public string Message { get; }

        public static implicit operator EstimatingPaymentErrorModel((InvoicePaymentSettlementModeErrorModel, string) error) => new EstimatingPaymentErrorModel(error.Item1, error.Item2);
    }
}
