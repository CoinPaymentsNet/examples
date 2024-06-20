<?php

namespace CoinPaymentsApiExamples\Helpers;

use Dotenv\Dotenv;

class EnvHelper
{
    public static function loadEnv(string $path): void
    {
        $dotenv = Dotenv::createImmutable($path);
        $dotenv->load();
    }

    public static function get(string $key)
    {
        $value = $_ENV[$key] ?? null;
        if (is_null($value)) {
            throw new \Exception(sprintf('Could not get parameter "%s" from environment values.', $key));
        }

        return $value;
    }
}