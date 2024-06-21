<?php
include dirname(__DIR__, 2) . '/vendor/autoload.php';

use CoinPaymentsApiExamples\Helpers\ApiHelper;
use CoinPaymentsApiExamples\Helpers\EnvHelper;

EnvHelper::loadEnv(dirname(__DIR__, 2));

/**
 * Let's assume that we already have wallet in LTCT with some positive balance greater than $100 (in equivalent).
 * Goals of this example:
 *  - create second wallet
 *  - withdraw $100 worth of LTCT from the first wallet and send it to the second wallet
 */

$apiHelper = new ApiHelper(EnvHelper::get('CLIENT_ID'), EnvHelper::get('CLIENT_SECRET'));

// Obtain currencies IDs
$currencies = $apiHelper->sendApiRequest('GET', '/api/v1/currencies?q=LTCT');
$ltctCurrencyId = $currencies[0]['id'];

$currencies = $apiHelper->sendApiRequest('GET', '/api/v1/currencies?q=USD&types=fiat');
$usdCurrencyId = $currencies[0]['id'];

// Obtain currency info
$ltctCurrency = $apiHelper->sendApiRequest('GET', '/api/v1/currencies/' . $ltctCurrencyId);
$usdCurrency = $apiHelper->sendApiRequest('GET', '/api/v1/currencies/' . $usdCurrencyId);

// Create of second wallet
print "Creating of second wallet..." . PHP_EOL;

//$payload = [
//    "currencyId" => $currencyId,
//    "label" => "Test wallet #" . date("YmdHis"),
//];
//$response = $apiHelper->sendApiRequest('POST', '/api/v1/merchant/wallets', $payload);
//$secondWalletId = $response['walletId'];
$secondWalletId = 'bcc67aa7-ae0a-4d5f-bb4d-67bdc31c9755';
printf("Wallet %s has been created." . PHP_EOL, $secondWalletId);
print PHP_EOL;

# Obtain addresses list of second wallet
print "Obtaining of second wallet address..." . PHP_EOL;
$secondWalletAddressesEndpoint = sprintf('/api/v1/merchant/wallets/%s/addresses', $secondWalletId);
$secondWalletAddresses = $apiHelper->sendApiRequest('GET', $secondWalletAddressesEndpoint);
if (empty($secondWalletAddresses)) { // if there are no addresses yet, then create new one
    $payload = ['label' => 'Second Wallet Address #' . date("YmdHis"),];
    $response = $apiHelper->sendApiRequest('POST', $secondWalletAddressesEndpoint, $payload);
    $secondWalletAddress = $response['networkAddress'];
} else {
    $secondWalletAddress = $secondWalletAddresses[0]['address'];
}

printf("Second wallet address: %s" . PHP_EOL, $secondWalletAddress);
print PHP_EOL;

// We want to withdraw $100 worth of LTCT
$withdrawalAmount = 100;

// We need to convert amount value from USD to LTCT
$query = [
    'from' => $usdCurrencyId,
    'to' => $ltctCurrencyId,
];
$rates = $apiHelper->sendApiRequest('GET', '/api/v1/rates?' . http_build_query($query));
$rateData = $rates['items'][0];
$rate = $rateData['rate'];

// Withdrawal can be done in the smallest units of currency
// We need to calculate equivalent of $100 in the smallest units of LTCT
$withdrawalAmountInSmallestUnits = (int)($rate * $withdrawalAmount * pow(10, $ltctCurrency['decimalPlaces']));

// Obtain list of already existed wallets and find one that has enough on the balance
print "Searching wallet with positive balance grater that $100 ..." . PHP_EOL;

$wallets = $apiHelper->sendApiRequest('GET', '/api/v1/merchant/wallets');
$sourceWalletId = null;
foreach ($wallets as $wallet) {
    $walletId = $wallet['walletId'];
    if ($walletId == $secondWalletId) {
        continue;
    }

    $walletCurrencyId = (string)$wallet['currencyId'];
    if ($walletCurrencyId != $ltctCurrencyId) {
        continue;
    }

    $confirmedBalance = $wallet['confirmedBalance']['value']; // this is value in the smallest units
    if ($confirmedBalance < $withdrawalAmountInSmallestUnits) {
        continue;
    }


    $sourceWalletId = $walletId;
    break;
}

if (is_null($sourceWalletId)) {
    print "Wallet not found." . PHP_EOL;
    exit(1);
}

printf("Found wallet: %s" . PHP_EOL, $sourceWalletId);
print PHP_EOL;

print "Sending funds..." . PHP_EOL;
printf("Withdrawal amount: %s %s smallest units (equivalent of %s %s)" . PHP_EOL, $withdrawalAmountInSmallestUnits, $ltctCurrency['symbol'], $withdrawalAmount, $usdCurrency['symbol']);
printf("From wallet: %s" . PHP_EOL, $sourceWalletId);
printf("To address: %s" . PHP_EOL, $secondWalletAddress);

// Create spend request
$payload = [
    "toAddress" => $secondWalletAddress,
    "toCurrencyId" => $ltctCurrencyId, // currency ID of second wallet
    "amountInSmallestUnits" => (string)$withdrawalAmountInSmallestUnits,
];
$response = $apiHelper->sendApiRequest('POST', sprintf('/api/v1/merchant/wallets/%s/spend/request', $sourceWalletId), $payload);
$spendRequestId = $response['spendRequestId'];
printf("Request ID: %s" . PHP_EOL, $spendRequestId);

// Confirm spend request
$payload = ['spendRequestId' => $spendRequestId];
$response = $apiHelper->sendApiRequest('POST', sprintf('/api/v1/merchant/wallets/%s/spend/confirmation', $sourceWalletId), $payload);
$result = (bool)($response['result'] ?? false);

$result ? print "Confirmed!" : print "Failed!";
echo PHP_EOL;
exit;

