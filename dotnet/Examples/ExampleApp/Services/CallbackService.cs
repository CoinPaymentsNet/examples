using Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ExampleApp.Services
{
    public class CallbackService
    {
        public static ConcurrentStack<string> Notifications { get; set; } = new();
        /// <summary>
        /// It shows how the signing and validation required for triggering callback notifications is done and where to move these parameters. 
        /// Contains information about the properties and their values in the Notification object. 
        /// </summary>
        /// <param name="clientId">ClientId value of current merchant. ClientId is sent with `X-CoinPayments-Client` key in the request header</param>
        /// <param name="secret">ClientSecret value of current merchant</param>
        /// <param name="requestUrl">Endpoint address where callback notification is expected</param>
        /// <param name="expectedSignature">The signature generated to verify the request is sent with the `X-CoinPayments-Signature` key in the header.</param>
        /// <param name="timestampHeader">The timestamp generated to verify the request is sent with the `X-CoinPayments-Timestamp` key in the header. Indicates the date the request was created</param>
        /// <param name="notification">Refers to the values that can be included in a callback notification</param>
        /// <returns>
        /// <see langword="true"/> Creates a new notification if the expected signature and the generated signature are verified; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ValidateSignatureAndCreateCallback(string clientId, string secret,string requestUrl, string expectedSignature, string timestampHeader, NotificationDto notification)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                var body = JsonSerializer.Serialize(notification);
                writer.Write("POST");
                writer.Write(requestUrl);
                writer.Write(clientId);
                writer.Write(timestampHeader);
                writer.Write(body);
                writer.Flush();

                stream.Position = 0;

                var hashBytes = hmac.ComputeHash(stream);
                var hash = Convert.ToBase64String(hashBytes);

                Console.WriteLine($"{hash} - actual signature");
                Console.WriteLine($"{expectedSignature} - expected signature");
                var isValid = string.Equals(hash, expectedSignature);
                if (isValid) {
                    Notifications.Push(body);
                }
                return isValid ;
            }
        }
    }
}
