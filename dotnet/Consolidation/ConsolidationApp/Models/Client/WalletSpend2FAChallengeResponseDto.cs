namespace ConsolidationApp.Models.Client
{
    /// <summary>
    /// The response to a request to spend funds from a wallet
    /// </summary>
    public class WalletSpend2FAChallengeResponseDto
    {
        /// <summary>
        /// the id of the created request to spend funds
        /// </summary>
        public required string SpendRequestId { get; set; }

        /// <summary>
        /// additional validation token that must be sent up with the signed request
        /// </summary>
        public required string SpendRequestToken { get; set; }
    }
   
}
