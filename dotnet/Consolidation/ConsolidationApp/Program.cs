using ConsolidationApp.Models;
using ConsolidationApp.Models.Currencies;
using ConsolidationApp.UseCases;

var currentUser = new ClientEnvironmentModel()
{
    Email = "",// some_email_address@merchantmail.com
    Password = "",// cp**********82
    Id = "", // feda5ae***********7193226
    Secret = "", //F+mHut20m**************uo3wBPyJQX7/+VlKLA= 
    Environment = "STAGING",
    ToUseEndpoints = new() { 
        ORION_API_URL = "https://api-staging.coinpaymints.com", 
        ENVIRONMENT_NAME = "STAGING" }
};

var consolidator = new WalletConsolidator(currentUser);

await consolidator.CreatePermanentAddress_TransferFromMain_ConsolidateAndSpendETH(Currencies.ETH.Id);

