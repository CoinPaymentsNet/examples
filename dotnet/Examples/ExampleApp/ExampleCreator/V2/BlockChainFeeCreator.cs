using ExampleApp.Clients;

namespace ExampleApp.ExampleCreator.V2
{
    public class BlockChainFeeCreator(CoinPaymentsApiPublicClient client)
    {
        public async Task<string> GetBlockchainFeeByGivenCurrency(string currency) =>
            (await client.ExecuteAsync($"fees/blockchain/{currency}", HttpMethod.Get)).Content;
    }
}
