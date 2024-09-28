namespace  ConsolidationApp.Models.Client
{
    public class WalletSpendRequestResponseDto
    {
        public required string Id { get; set; }
        public Guid WalletId { get; set; }
        public required string BlockchainFee { get; set; }
        public required string CoinPaymentsFee { get; set; }
        public required string BlockchainFeeCurrency { get; set; }
        public required string CoinPaymentsFeeCurrency { get; set; }
        public required string FromCurrency { get; set; }
        public required string ToAmount { get; set; }
        public required string ToCurrency { get; set; }
        public required string ToAddress { get; set; }
        public required string WalletLabel { get; set; }
    }
}
