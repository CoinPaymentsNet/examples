<?php

function sendApiRequest(string $method, string $endpoint, array $payload = []): array
{
    $clientId     = '_YOUR_CLIENT_ID_HERE_';
    $clientSecret = '_YOUR_CLIENT_SECRET_HERE_';

    $payload = !empty($payload) ? json_encode($payload) : '';

    $apiUrl          = sprintf('https://api.coinpayments.com%s', $endpoint);
    $date            = (new DateTimeImmutable())->setTimezone(new DateTimeZone('UTC'))->format('Y-m-d\TH:i:s');
    $signatureString = implode('', [chr(239), chr(187), chr(191), $method, $apiUrl, $clientId, $date, $payload]);

    $headers = [
        'Content-Type'             => 'application/json',
        'X-CoinPayments-Client'    => $clientId,
        'X-CoinPayments-Timestamp' => $date,
        'X-CoinPayments-Signature' => base64_encode(hash_hmac('sha256', $signatureString, $clientSecret, true)),
    ];

    $guzzle  = new GuzzleHttp\Client(['verify' => false]);

    try {
        $res = $guzzle->request($method, $apiUrl, [
            'body'    => $payload,
            'headers' => $headers,
            'http_errors' => false,
        ]);

        $body = $res->getBody()->getContents();

        return json_decode($body, 1);
    } catch (GuzzleHttp\Exception\BadResponseException $e) {
        return ['error' => $e->getResponse()];
    }
}

function generateQRData($currencyId, $currencyName, $address, $amount, $tag = null): string
{
    $currencySuffix = current(array_reverse(explode(':', $currencyId)));
    switch ($currencyName) {
        case 'Bitcurrency SV':
            return sprintf("bitcurrency:%s?sv&amount=%s", $address, $amount);
        case 'Tether USD (Omni)':
            return sprintf("bitcurrency:%s?amount=%s&req-asset=%s", $address, $amount, $currencyId);
        case 'BinanceCoin':
            return sprintf("bnb:%s?sv&amount=%s&req-asset=%s", $address, $amount, $currencyId);
        case 'Tether USD (ERC20)':
            return sprintf("ethereum:%s?value=%s&req-asset=%s", $address, $amount, $currencySuffix);
        case 'Tether USD (TRC20)':
            return sprintf("tron:%s?value=%s&req-asset=%s", $address, $amount, $currencySuffix);
        default:
            $currencyName = str_replace(" ", "", strtolower($currencyName));
            return sprintf("%s:%s?amount=%s%s", $currencyName, $address, $amount, !empty($tag) ? '&tag=' . $tag : '');
    }
}

$currencyId = "5057"; // USD - Note: you should use fiat currency here
$price = 123.45; // Price in fiat
$quantity = 1;

$payload = [
    "clientId" => '_YOUR_CLIENT_ID_HERE_',
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
        "refundEmail" => "info@test.com"
    ]
];

// Create invoice API call
$response = sendApiRequest('POST', '/api/v1/merchant/invoices', $payload);
if (isset($response['error'])) {
    printf("Error: %s\n", $response['error']);
    exit(1);
}

if (empty($response['invoices'])) {
    printf("Error: Could not retrieve invoices.\n");
    exit(1);
}

$invoice = current($response['invoices']);
$invoiceId = $invoice['id'];
$paymentCurrencies = $invoice['payment']['paymentCurrencies'] ?? [];

// Note: you can build your own currency selector if you allow your customers to choose cryptocurrency to pay with
// For example, you can build simple select like this:
//    $paymentCurrenciesInfo = [];
//    foreach ($paymentCurrencies as $paymentCurrency) {
//        $currencyId = $paymentCurrency['currency']['id'];
//        $paymentCurrenciesInfo[$currencyId] = [
//            'currencyName' => $paymentCurrency['currency']['name'],
//            'currencySymbol' => $paymentCurrency['currency']['symbol'],
//            'displayValue' => $paymentCurrency['amount']['displayValue'],
//        ];
//    }
//
//    echo '<select name="currencyId">';
//    foreach ($paymentCurrenciesInfo as $currencyId => $currencyInfo) {
//        echo sprintf(
//            '<option value="%s">%s %s (%s)</option>',
//            $currencyId,
//            $currencyInfo['displayValue'],
//            $currencyInfo['currencyName'],
//            $currencyInfo['currencySymbol']
//        );
//    }
//    echo '</select>';

// if you want to use a specific currency and not let your customers choose - this requires you to have KYC/KYB verification passed
// $currencyId = "9:TR7NHqjeKQxGTCi8q8ZY4pL8otSzgjLj6t"; // i.e. USDT.TRC20
$currencyId = "1002"; // i.e. LTCT - for those who have not passed KYC/KYB

$endpoint = sprintf('/api/v1/invoices/%s/payment-currencies/%s', $invoiceId, $currencyId);

$response = sendApiRequest('GET', $endpoint);

// Note: you should build your own checkout page and display address, amount, currency and transaction expiration time returned in response
$address       = $response['addresses']['address'];
$amount        = $response['amount']['displayValue'];
$currency      = sprintf('%s (%s)', $response['currency']['name'], $response['currency']['symbol']);
$trxExpiration = $response['expires'];

// it's just a simple representation of data form response
echo "Please complete funds sending:\n";
echo sprintf("%s %s\n", $amount, $currency);
echo sprintf("Address: %s\n", $address);
echo sprintf("Transaction will expire at: %s\n", $trxExpiration);


// Note: You also can generate QR-code for this invoice
// Here is sample code for this using "endroid/qr-code" library (you are free to use any QR-code generation library of your choice)

use Endroid\QrCode\Color\Color;
use Endroid\QrCode\QrCode;
use Endroid\QrCode\Writer\PngWriter;

// Create a new QR code instance
$qrData = generateQRData($response['currency']['id'], $response['currency']['name'], $address, $amount);
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
$result->saveToFile(__DIR__.'/qrcode.png');


// Note: CoinPayments API offers webhook notifications, a powerful feature that allows merchants to seamlessly enable
// and manage notifications sent from CoinPayments API to their own merchant API when specific events occur.
// This provides merchants with real-time updates on important activities within their CoinPayments account.

// Here you will find IPN request example
//{
//  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
//  "type": "invoiceCreated",
//  "timestamp": "2023-04-07T06:58:19.9798764+00:0",
//  "invoice": {
//    "invoiceId": "string",
//    "id": "string",
//    "userId": "string",
//    "merchantId": "string",
//    "merchantClientId": "string",
//    "invoiceNumber": "string",
//    "invoiceNumberSuffix": "string",
//    "createdAt": "string",
//    "invoiceDate": "string",
//    "dueDate": "string",
//    "description": "string",
//    "expiresDate": "string",
//    "customData": {
//      "additionalProp1": "string",
//      "additionalProp2": "string",
//      "additionalProp3": "string"
//    },
//    "notes": "string",
//    "notesToRecipient": "string",
//    "buyerDataCollectionMessage": "string",
//    "termsAndConditions": "string",
//    "metadata": {
//      "integration": "string",
//      "hostname": "string"
//    },
//    "poNumber": "string",
//    "buyer": {
//      "companyName": "string",
//      "email": "string",
//      "firstName": "string",
//      "lastName": "string",
//      "phoneNumber": "string",
//      "address": {
//        "address1": {
//          "address1": null
//        },
//        "address2": {
//          "address2": null
//        },
//        "address3": {
//          "address3": null
//        },
//        "provinceOrState": {
//          "provinceOrState": null
//        },
//        "city": {
//          "city": null
//        },
//        "suburbOrDistrict": {
//          "suburbOrDistrict": null
//        },
//        "countryCode": {
//          "countryCode": null
//        },
//        "postalCode": {
//          "postalCode": null
//        }
//      }
//    },
//    "shipping": {
//      "method": "string",
//      "companyName": "string",
//      "name": {
//        "firstName": {
//          "firstName": null
//        },
//        "lastName": {
//          "lastName": null
//        }
//      },
//      "emailAddress": "string",
//      "phoneNumber": "string",
//      "address": {
//        "address1": {
//          "address1": null
//        },
//        "address2": {
//          "address2": null
//        },
//        "address3": {
//          "address3": null
//        },
//        "provinceOrState": {
//          "provinceOrState": null
//        },
//        "city": {
//          "city": null
//        },
//        "suburbOrDistrict": {
//          "suburbOrDistrict": null
//        },
//        "countryCode": {
//          "countryCode": null
//        },
//        "postalCode": {
//          "postalCode": null
//        }
//      }
//    },
//    "lineItems": {
//      "amount": "string",
//      "customId": "string",
//      "description": "string",
//      "name": "string",
//      "originalAmount": "string",
//      "quantity": 0,
//      "sku": "string",
//      "tax": "string",
//      "type": "string"
//    },
//    "merchantOptions": {
//      "showAddress": false,
//      "showPhone": false,
//      "showRegistrationNumber": false,
//      "showEmail": false,
//      "additionalInfo": {
//        "additionalInfo": null
//      }
//    },
//    "emailDeliveryOptions": {
//      "to": "string",
//      "cc": "string",
//      "bcc": "string"
//    },
//    "amount": {
//      "currencyId": 0,
//      "subtotal": "string",
//      "shippingTotal": "string",
//      "handlingTotal": "string",
//      "discountTotal": "string",
//      "taxTotal": "string",
//      "total": "string"
//    },
//    "state": "string",
//    "flags": {
//      "requireBuyerNameAndEmail": true,
//      "sendPaymentCompleteEmailNotification": true,
//      "isPos": true
//    },
//    "canceledAt": 0,
//    "completedAt": 0,
//    "confirmedAt": 0,
//    "payments": [
//      {
//        "id": "string",
//        "invoiceId": "string",
//        "createdAt": 0,
//        "expiresAt": 0,
//        "cancelledAt": 0,
//        "detectedAt": 0,
//        "pendingAt": 0,
//        "confirmedAt": 0,
//        "completedAt": 0,
//        "scheduledAt": 0,
//        "state": "string",
//        "refundedAt": 0,
//        "refundEmail": "string",
//        "isGuest": true,
//        "hotWallet": {
//          "nativeCurrency": {
//            "id": 0,
//            "smartContract": "string"
//          },
//          "paymentSubTotalInNativeCurrency": 0,
//          "merchantMarkupOrDiscountInNativeCurrency": 0,
//          "buyerFeeInNativeCurrency": {
//            "coinPaymentsFee": 0,
//            "networkFee": 0,
//            "conversionFee": 0
//          },
//          "merchantFeeInNativeCurrency": {
//            "coinPaymentsFee": 0,
//            "networkFee": 0,
//            "conversionFee": 0
//          },
//          "confirmedAmountInNativeCurrency": 0,
//          "unconfirmedAmountInNativeCurrency": 0,
//          "id": "string",
//          "paymentId": "string",
//          "currency": {
//            "id": 0,
//            "smartContract": {}
//          },
//          "merchantPayoutCurrency": {
//            "id": 0,
//            "smartContract": {}
//          },
//          "currencyRateFromInvoiceCurrency": 0,
//          "paymentReceiveAddress": "string",
//          "merchantPayoutAddress": "string",
//          "merchantPayoutWalletId": "string",
//          "paymentSubTotal": "string",
//          "merchantMarkupOrDiscount": 0,
//          "isConversion": true,
//          "buyerFee": {
//            "coin_payments_fee": 0,
//            "network_fee": 0,
//            "conversion_fee": 0
//          },
//          "merchantFee": {
//            "coin_payments_fee": 0,
//            "network_fee": 0,
//            "conversion_fee": 0
//          },
//          "payoutFrequency": "string",
//          "createdAt": 0,
//          "error": {
//            "code": "string",
//            "message": [
//              "Unknown = 0",
//              "NegativeRate = 1",
//              "PayoutAddressIsNull = 2",
//              "PaymentSubTotalIsLessThanMerchantTotalFee = 4",
//              "TotalBuyerWillPayIsNegativeOrZero = 8",
//              "TotalBuyerWillPayIsLessThanBuyerNetworkFee = 16",
//              "TotalMerchantFeeRatioIsMoreThanMaximumRatioSetting = 32",
//              "PayoutAmountIsLessThanDust = 64",
//              "CurrencyIsNotActive = 128",
//              "AmountIsBelowOfConversionLimit = 256",
//              "AmountIsAboveOfConversionLimit = 512",
//              "UserLimitIsReached = 1024",
//              "NotEnoughToActivateRippleAddress = 2048",
//              "ConversionPairDoesNotExist = 4096",
//              "AddressIsNotValid = 8_192",
//              "DoesNotHaveCompletedKyc = 16_384",
//              "UnstoppableDomainNotFound = 32_768",
//              "UnstoppableDomainNotFoundForCurrency = 65_536",
//              "UserWalletIsLocked = 131_072"
//            ]
//          },
//          "confirmations": 0,
//          "confirmedAmount": 0,
//          "requiredConfirmations": 0,
//          "unconfirmedAmount": 0,
//          "assignment": {
//            "assignedFrom": "string",
//            "assignedUntil": "string",
//            "completedDate": "string"
//          },
//          "pooledWalletId": "string",
//          "expiresAt": 0
//        },
//        "payout": {
//          "destinationAmountInNativeCurrency": 0,
//          "payoutAmountToMerchantInNativeCurrency": 0,
//          "buyerBlockchainFeeAfterGroupingInNativeCurrency": 0,
//          "merchantBlockchainFeeAfterGroupingInNativeCurrency": 0,
//          "id": "string",
//          "invoicePaymentId": "string",
//          "invoicePaymentHotWalletId": "string",
//          "created": "string",
//          "sent": "string",
//          "confirmed": "string",
//          "failed": "string",
//          "merchantPayoutWalletId": "string",
//          "merchantPayoutWalletCurrencyId": 0,
//          "merchantPayoutWalletSmartContract": {},
//          "merchantPayoutAddress": "string",
//          "payoutAmountToMerchant": 0,
//          "blockchainTransactionId": "string",
//          "state": "string",
//          "batchId": "string",
//          "destinationAmount": 0,
//          "transactionId": 0,
//          "buyerBlockchainFeeAfterGrouping": 0,
//          "merchantBlockchainFeeAfterGrouping": 0
//        },
//        "refund": {
//          "payoutAmountInNativeCurrency": 0,
//          "payoutNetworkFeesInNativeCurrency": 0,
//          "estimatedNetworkFeesInNativeCurrency": 0
//        },
//        "isActive": true
//      }
//    ],
//    "payoutConfig": {
//      "currencyId": {
//        "id": 0,
//        "smart_contract": "string"
//      },
//      "address": "string",
//      "wallet_id": "string",
//      "frequency": [
//        "normal",
//        "asSoonAsPossible",
//        "hourly",
//        "nightly",
//        "weekly"
//      ]
//    },
//    "partialAcceptAvailable": true
//  }
//}

echo PHP_EOL;
