<?php

namespace CoinPaymentsApiExamples\Helpers;

use GuzzleHttp\Client;
use GuzzleHttp\Exception\BadResponseException;

class ApiHelper
{
    const API_URL = 'https://api.coinpayments.net';

    /** @var string */
    private $clientId;

    /** @var string */
    private $clientSecret;

    public function __construct(string $clientId, string $clientSecret)
    {
        $this->clientId = $clientId;
        $this->clientSecret = $clientSecret;
    }

    public function sendApiRequest(string $method, string $endpoint, array $payload = []): array
    {
        $payload = !empty($payload) ? json_encode($payload) : '';
        $apiUrl  = sprintf('%s%s', self::API_URL, $endpoint);
        $guzzle  = new Client(['verify' => false]);
        try {
            $response = $guzzle->request($method, $apiUrl, [
                'body'    => $payload,
                'headers' => $this->buildHeaders($method, $apiUrl, $payload),
                'http_errors' => false,
            ]);

            $body = $response->getBody()->getContents();
            if (!empty($body)) {
                return json_decode($body, true);
            }

            $statusCode = $response->getStatusCode();

            return ['result' => $statusCode >= 200 && $statusCode < 300];
        } catch (BadResponseException $e) {
            return ['error' => $e->getResponse()];
        }
    }

    private function buildHeaders(string $method, string $url, string $payload): array
    {
        $date            = (new \DateTimeImmutable())->setTimezone(new \DateTimeZone('UTC'))->format('Y-m-d\TH:i:s');
        $signatureString = implode('', [chr(239), chr(187), chr(191), $method, $url, $this->clientId, $date, $payload]);

        return [
            'Content-Type'             => 'application/json',
            'X-CoinPayments-Client'    => $this->clientId,
            'X-CoinPayments-Timestamp' => $date,
            'X-CoinPayments-Signature' => base64_encode(hash_hmac('sha256', $signatureString, $this->clientSecret, true)),
        ];

    }
}