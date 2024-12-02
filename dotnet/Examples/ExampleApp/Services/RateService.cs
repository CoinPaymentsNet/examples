using ExampleApp.Clients;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleApp.Services
{
    public class RateService(CoinPaymentsApiClient _prodClient)
    {
        public async Task<decimal> GetUSDRateSmallestUnitByCurrencyId(string from, decimal usdAmount = 1_00)
        {
            var amount = Currencies.ToFormattedTokenValueFromSmallestUnit(Currencies.USD.DecimalPlaces, usdAmount);
            var oneUsdByCurrency = await _prodClient.ExecuteAsync<Paged<RateDto>>($"rates?from={Currencies.USD.Id}&to={from}", HttpMethod.Get);
            if (oneUsdByCurrency == null || !oneUsdByCurrency.Items.Any() ||
                Convert.ToDecimal(oneUsdByCurrency.Items[0].Rate) <= 0)
                throw new Exception("No USD rates found");
            return Math.Truncate(Convert.ToDecimal(
                Currencies.CoinValueFromDisplayUnit(
                    Currencies.All[Convert.ToInt32(from)].Id,
                    amount * Convert.ToDecimal(oneUsdByCurrency.Items[0].Rate)
                )));
        }
       
    }
}
