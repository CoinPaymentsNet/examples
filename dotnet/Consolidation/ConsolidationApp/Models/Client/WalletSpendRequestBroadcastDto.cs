namespace ConsolidationApp.Models.Client
{
    /// <summary>
    /// Client signed spend request
    /// </summary>
    public class WalletSpendRequestBroadcastDto
    {
        /// <summary>
        /// the validation token from the original request
        /// </summary>
        public required string SpendRequestToken { get; set; }

        /// <summary>
        /// the client signed transaction encoded in hex
        /// </summary>
        public required string SignedTxHex { get; set; }

        /// <summary>
        /// 2fa token
        /// </summary>
        public required string Otp { get; set; }
    }
}
