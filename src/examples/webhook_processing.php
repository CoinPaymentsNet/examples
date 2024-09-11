<?php

include dirname(__DIR__, 2) . '/vendor/autoload.php';

use CoinPaymentsApiExamples\Helpers\ApiHelper;
use CoinPaymentsApiExamples\Helpers\EnvHelper;

EnvHelper::loadEnv(dirname(__DIR__, 2));

$apiHelper = new ApiHelper(EnvHelper::get('CLIENT_ID'), EnvHelper::get('CLIENT_SECRET'));

// Get raw payload of webhook (it will be raw JSON)
$content = file_get_contents('php://input');

// One of possible ways to get current URL (URL where webhook was sent)
$protocol = (empty($_SERVER['HTTPS']) ? 'http' : 'https');
$currentUrl = sprintf("%s://%s%s", $protocol, $_SERVER['HTTP_HOST'], $_SERVER['REQUEST_URI']);
// another way:
// Get list of all webhooks for particular pair of "Client ID" and "Client Secret" using this API endpoint
// https://docs.coinpayments.com/#operation/getMerchantWebhooks
// It will return JSON with list of all webhooks
// URL can be detected by using of type returned in webhook payload (i.e. "invoicePaid") and
// notifications assigned for particular webhook from the list

// Get webhook signature sent by CoinPayments
$receivedSignature = $_SERVER['HTTP_X_COINPAYMENTS_SIGNATURE'];

// Get date and time of webhook sent by CoinPayments
$date = $_SERVER['HTTP_X_COINPAYMENTS_TIMESTAMP'];

// Method always should be POST
$method = 'POST';

$expectedSignature = $apiHelper->calculateSignature($method, $currentUrl, $content, $date);

// Compare expected and received signatures
if ($receivedSignature !== $expectedSignature) {
    throw new Exception('Invalid signature');
}

// Process webhook content at your own