<?php

include dirname(__DIR__, 3) . '/vendor/autoload.php';

use CoinPaymentsApiExamples\Helpers\ApiHelper;
use CoinPaymentsApiExamples\Helpers\EnvHelper;

EnvHelper::loadEnv(dirname(__DIR__, 3));

$apiHelper = new ApiHelper(EnvHelper::get('CLIENT_ID'), EnvHelper::get('CLIENT_SECRET'));

// Obtain currencies IDs
$currencies = $apiHelper->sendApiRequest('GET', '/api/v1/currencies?q=ETH&types=crypto');
$ethCurrencyId = $currencies[0]['id'];

$currencies = $apiHelper->sendApiRequest('GET', '/api/v1/currencies?q=TUSD.ERC20&types=token');
// Note: this is an important part: This information is returned in the format 'coinID:smartContract'.
$tusdCurrencyId = $currencies[0]['id'];

// We need to separate coinId and smartContract and pass them in appropriate parameters in "/api/v1/merchant/wallets" API call
list ($tusdCurrencyIdNumber, $tusdContractAddress) = array_slice(explode(':', $tusdCurrencyId), 0, 2);


// Create first wallet in ETH
print "Creating of first wallet..." . PHP_EOL;
$payload = [
    "currencyId" => $ethCurrencyId,
    "label" => "Test wallet ETH #" . date("YmdHis"),
];
$response = $apiHelper->sendApiRequest('POST', '/api/v1/merchant/wallets', $payload);
$ethWalletId = $response['walletId'];
printf("Wallet %s has been created (currency ETH)." . PHP_EOL, $ethWalletId);
print PHP_EOL;


// Create second wallet in TUSD.ERC20
print "Creating of second wallet..." . PHP_EOL;
$payload = [
    "currencyId" => $tusdCurrencyIdNumber,
    "label" => "Test wallet TUSD.ERC20 #" . date("YmdHis"),
];

is_null($tusdContractAddress) ?: $payload['contractAddress'] = $tusdContractAddress;
$response = $apiHelper->sendApiRequest('POST', '/api/v1/merchant/wallets', $payload);
$ethWalletId = $response['walletId'];
printf("Wallet %s has been created (currency TUSD.ERC20)." . PHP_EOL, $ethWalletId);
print PHP_EOL;