<?php

include dirname(__DIR__, 3) . '/vendor/autoload.php';

use CoinPaymentsApiExamples\Helpers\ApiHelper;
use CoinPaymentsApiExamples\Helpers\EnvHelper;
use CoinPaymentsApiExamples\Helpers\QrCodeHelper;
use Endroid\QrCode\Color\Color;
use Endroid\QrCode\QrCode;
use Endroid\QrCode\Writer\PngWriter;

EnvHelper::loadEnv(dirname(__DIR__, 3));

$apiHelper = new ApiHelper(EnvHelper::get('CLIENT_ID'), EnvHelper::get('CLIENT_SECRET'));

$currencyId = "5057"; // USD - original currency of payment
$paymentCurrencyId = "1002"; // LTCT
// You can also obtain payment currency ID with following code:
// $currencies = $apiHelper->sendApiRequest('GET', '/api/v1/currencies?q=LTCT');
// $paymentCurrencyId = $currencies[0]['id'];

$price = 1.45;
$quantity = 1;

$payload = [
    "clientId" => EnvHelper::get('CLIENT_ID'),
    "invoiceId" => "3432432432",
    "items" => [
        [
            "name" => "Item Test Name",
            "quantity" => [
                "value" => $quantity,
                "type" => 2 // type of the sold product, can be "1" = "hours" or "2" = "units"
            ],
            "originalAmount" => [
                "currencyId" => $currencyId,
                "value" => $price * 100, // monetary value as an integer in the base (smallest) unit of the currency (i.e. cents)
            ],
            "amount" => [
                "currencyId" => $currencyId,
                "value" => $quantity * $price * 100,
            ],
        ]
    ],
    "amount" => [
        "currencyId" => $currencyId,
        "value" => $quantity * $price * 100, // represents the total value of all bought items in the original currency
    ],
    "payment" => [
        "refundEmail" => "info@test.com",
        "paymentCurrency" => $paymentCurrencyId
    ]
];

// Create invoice API call
$response = $apiHelper->sendApiRequest('POST', '/api/v1/merchant/invoices', $payload);

if (isset($response['error'])) {
    printf("Error: %s\n", $response['error']);
    exit(1);
}

if (empty($response['invoices'])) {
    printf("Error: Could not retrieve invoices.\n");
    exit(2);
}

$invoice = current($response['invoices']);
if (empty($hotWallet = $invoice['hotWallet'] ?? null)) {
    printf("Error: Could not retrieve hot wallet.\n");
    exit(1);
}

// Note: you should build your own checkout page and display address, amount, currency and transaction expiration time returned in response
$address       = $hotWallet['addresses']['address'];
$amount        = $hotWallet['amount']['displayValue'];
$currency      = sprintf('%s (%s)', $hotWallet['currency']['name'], $hotWallet['currency']['symbol']);
$trxExpiration = $hotWallet['expires'];

// it's just a simple representation of data form response
echo "Please complete funds sending:\n";
echo sprintf("%s %s\n", $amount, $currency);
echo sprintf("Address: %s\n", $address);
echo sprintf("Transaction will expire at: %s\n", $trxExpiration);


// Note: You also can generate QR-code for this invoice
// Here is sample code for this using "endroid/qr-code" library (you are free to use any QR-code generation library of your choice)

$qrCodeHelper = new QrCodeHelper();
$qrData = $qrCodeHelper->generateQRData($hotWallet['currency']['id'], $hotWallet['currency']['name'], $address, $amount);

// Create a new QR code instance
$qrCode = new QrCode($qrData);

// Set the QR code dimensions
$qrCode->setSize(200);

// Set the QR code colors
$qrCode->setForegroundColor(new Color(23, 94, 213)); // #2A5ED5
$qrCode->setBackgroundColor(new Color(0, 0, 0)); // #000000

// Set the QR code margin
$qrCode->setMargin(5);

$writer = new PngWriter();
$result = $writer->write($qrCode);
$result->saveToFile(__DIR__ . '/qrcode.png');


// Note: CoinPayments API offers webhook notifications, a powerful feature that allows merchants to seamlessly enable
// and manage notifications sent from CoinPayments API to their own merchant API when specific events occur.
// This provides merchants with real-time updates on important activities within their CoinPayments account.

echo PHP_EOL;
