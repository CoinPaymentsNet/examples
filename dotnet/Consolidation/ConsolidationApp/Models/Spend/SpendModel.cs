using System.ComponentModel.DataAnnotations;

namespace ConsolidationApp.Models.Spend
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
	public class WalletSpendRequestDto
	{
		/// <summary>
		/// the list of recipients to send funds to
		/// </summary>
		public required WalletSpendRequestRecipientDto[] Recipients { get; set; }

		/// <summary>
		/// Custom memo to attach to this transaction, this will only be visible within CoinPayments®
		/// </summary>
		[StringLength(200)]
		public string? Memo { get; set; }

		/// <summary>
		/// Optional contract address for spending tokens instead of native coins for currencies that support smart contracts.
		/// </summary>
		/// <example>
		/// To send Tether USD tokens on Ethereum chain contract address should be set to 0xdac17f958d2ee523a2206206994597c13d831ec7
		/// as per https://etherscan.io/token/0xdac17f958d2ee523a2206206994597c13d831ec7
		/// If contract address is not specified native coins (ETH in this case) will be spent.
		/// Note that for both tokens and native coins the amount to send is specified in smallest atomic units
		/// </example>
		[StringLength(200)]
		public string? ContractAddress { get; set; }

		/// <summary>
		/// Currency into which funds should be converted.
		/// If at least one of the following
		/// a. <see cref="ToCurrency"/> is specified and not equal to currency of the wallet initiating the transfer
		/// b. <see cref="ContractAddress"/> is not equal to <see cref="ToContractAddress"/>
		/// is true the transfer is considered a conversion, otherwise it is considered a regular transfer
		/// </summary>
		/// <example>
		/// Wallet currency is 1 (BTC), <see cref="ToCurrency"/> is 2 (LTC). The transfer is considered a conversion
		/// </example>
		/// <example>
		/// Wallet currency is 1 (BTC), <see cref="ToCurrency"/> is not specified. This is considered a regular transfer
		/// </example>
		/// <example>
		/// Wallet currency is 4 (ETH), <see cref="ToCurrency"/> is 4 (ETH),
		/// <see cref="ContractAddress" /> is "0xdac17f958d2ee523a2206206994597c13d831ec7" (Tether USD), <see cref="ToContractAddress"/> is not set.
		/// This is considered a conversion from TetherUSD to ETH
		/// </example>
		/// <example>
		/// Wallet currency is 4 (ETH), <see cref="ToCurrency"/> is 4 (ETH),
		/// <see cref="ContractAddress" /> is not set, <see cref="ToContractAddress"/> is "0xdac17f958d2ee523a2206206994597c13d831ec7" (Tether USD).
		/// This is considered a conversion from ETH to TetherUSD
		/// </example>
		/// <example>
		/// Wallet currency is 1 (BTC), <see cref="ToCurrency"/> is 4 (ETH),
		/// <see cref="ContractAddress" /> is "0xdac17f958d2ee523a2206206994597c13d831ec7" (Tether USD), <see cref="ToContractAddress"/> is not set.
		/// This is an invalid request since BTC does not support smart contracts
		/// </example>
		public int? ToCurrency { get; set; }

		/// <summary>
		/// Address of the contract of the token to convert to.
		/// If at least one of the following
		/// a. <see cref="ToCurrency"/> is specified and not equal to currency of the wallet initiating the transfer
		/// b. <see cref="ContractAddress"/> is not equal to <see cref="ToContractAddress"/>
		/// is true the transfer is considered a conversion, otherwise it is considered a regular transfer
		/// </summary>
		/// <example>
		/// Sample scenarios:
		/// 1. Wallet currency is 1 (BTC), <see cref="ToCurrency"/> is 2 (LTC). The transfer is considered a conversion
		/// 2. Wallet currency is 1 (BTC), <see cref="ToCurrency"/> is not specified. This is considered a regular transfer
		/// 3. Wallet currency is 4 (ETH), <see cref="ToCurrency"/> is 4 (ETH),
		/// <see cref="ContractAddress" /> is "0xdac17f958d2ee523a2206206994597c13d831ec7" (Tether USD), <see cref="ToContractAddress"/> is not set.
		/// This is considered a conversion from TetherUSD to ETH
		/// 4. Wallet currency is 4 (ETH), <see cref="ToCurrency"/> is 4 (ETH),
		/// <see cref="ContractAddress" /> is not set, <see cref="ToContractAddress"/> is "0xdac17f958d2ee523a2206206994597c13d831ec7" (Tether USD).
		/// This is considered a conversion from ETH to TetherUSD
		/// 5. Wallet currency is 1 (BTC), <see cref="ToCurrency"/> is 4 (ETH),
		/// <see cref="ContractAddress" /> is "0xdac17f958d2ee523a2206206994597c13d831ec7" (Tether USD), <see cref="ToContractAddress"/> is not set.
		/// This is an invalid request since BTC does not support smart contracts
		/// </example>
		public string? ToContractAddress { get; set; }

		/// <summary>
		/// Specifies that fees would be subtracted from the amount being sent instead of being added to it
		/// </summary>
		/// <example>
		/// Suppose the amount to send is 10 DOGE and fee is 1 DOGE.
		/// If <see cref="FeeSubtractedFromAmount"/> is false (default) the total amount charged will be 11 DOGE, receiver will get 10 DOGE as requested.
		/// If <see cref="FeeSubtractedFromAmount"/> is true the total amount charged will be 10 DOGE, receiver will get 9 DOGE because 1 DOGE will be charged as fee
		/// </example>
		public bool FeeSubtractedFromAmount { get; set; }
	}
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
