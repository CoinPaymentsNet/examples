using System.ComponentModel.DataAnnotations;

namespace ConsolidationApp.Models.Client
{
    /// <summary>
    /// Request to create a transaction and spend funds from a wallet
    /// </summary>
    public class WalletSpendRequest2FADto
    {
        /// <summary>
        /// the list of addresses to send funds to
        /// </summary>
        public required string[] Addresses { get; set; }
        public required Guid WalletId { get; set; }

        public string? WalletLabel { get; set; }

        public string? WithdrawalDisplayAmount { get; set; }
    }
    
}