namespace ConsolidationApp.Models.Client
{
    /// <summary>
    /// Encapsulates an error from the wallet spend request validator
    /// </summary>
    public class SpendRequestValidateResponseErrorDto
    {
        /// <summary>
        /// the code for the error
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// the description for the error
        /// </summary>
        public required string Description { get; set; }
    }
}
