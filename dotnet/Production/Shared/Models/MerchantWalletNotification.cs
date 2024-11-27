using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Models
{
    public record NotificationDto(
        Guid WalletId,
        string? Address,
        Guid TransactionId,
        string? TxHash,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    Guid? SpendRequestId,
        [property: JsonConverter(typeof(JsonStringEnumConverter<WalletTransactionType>))]
    WalletTransactionType TransactionType,
        string Amount,
        string Symbol,
        string CoinPaymentsFee,
        string CoinPaymentsFeeSymbol,
        string BlockchainFee,
        string BlockchainFeeSymbol,
        string TotalAmount,
        string TotalAmountSymbol,
        string NativeAmount,
        string CoinPaymentsFeeNativeAmount,
        string BlockchainFeeNativeAmount,
        string TotalNativeAmount,
        string NativeSymbol,
        int Confirmations,
        int RequiredConfirmations);
}
