using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleApp.Clients
{
    public partial class CoinPaymentsApiClient
    {
        public Task<Paged<RateDto>> GetRates(string from, string to)
        {
            return ExecuteAsync<Paged<RateDto>>($"{API_URL}/rates?from={from}&to={to}", HttpMethod.Get);
        }
    }
}
