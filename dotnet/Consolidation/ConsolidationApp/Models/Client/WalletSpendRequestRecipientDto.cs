using System.ComponentModel.DataAnnotations;

namespace  ConsolidationApp.Models.Client
{
    /// <summary>
    /// Recipient of a transaction
    /// </summary>
    public class WalletSpendRequestRecipientDto
    {
        /// <summary>
        /// the address of the recipients account/wallet
        /// </summary>
        [Required]
        public required string Address { get; set; }

        /// <summary>
        /// the amount of money (in the currencies smallest unit, e.g. Satoshis for BTC) to send to the recipient address
        /// </summary>
        /// <example>
        /// BTC has 8 decimals which means 1 BTC is equal to 100_000_000 Satoshis.
        /// Therefore to send 0.00123 BTC you should specify amount to be 123_000 (0.00123 * 100_000_000) 
        /// </example>
        [Required]
        public required string Amount { get; set; }

        /// <summary>
        /// <see cref="Amount"/> as a <see cref="decimal"/>
        /// </summary>
        public decimal AmountAsDecimal => decimal.Parse(Amount);

        /// <summary>
        /// the amount of money (in the currencies smallest unit, e.g. Satoshis for BTC) to pay as a blockchain network fee
        /// </summary>
        public string? BlockChainFee { get; set; }

        /// <summary>
        /// <see cref="BlockChainFee"/> as a <see cref="decimal"/>
        /// </summary>
        public decimal? BlockChainFeeAsDecimal => BlockChainFee == null ? null : decimal.Parse(BlockChainFee);
    }
}
