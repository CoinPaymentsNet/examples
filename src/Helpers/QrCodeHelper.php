<?php

namespace CoinPaymentsApiExamples\Helpers;

class QrCodeHelper
{
    public function generateQRData($currencyId, $currencyName, $address, $amount, $tag = null): string
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
}