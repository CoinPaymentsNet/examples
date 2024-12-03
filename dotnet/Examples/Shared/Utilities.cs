using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared
{
    public class Utilites
    {
        public sealed class NumericConverter<T> : JsonConverter<T> where T : IParsable<T>, INumber<T>
        {
            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return T.Zero;

                if (reader.TokenType == JsonTokenType.String)
                {
                    var str = reader.GetString();
                    if (T.TryParse(str, provider: null, out var x))
                        return x;
                }
                if (reader.TokenType == JsonTokenType.Number)
                {
                    var str = reader.GetDecimal().ToString();
                    if (T.TryParse(str, provider: null, out var x))
                        return x;
                }
                return T.Zero;
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, value, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
        }
    }
    public static class Extensions
    {
        public static string AsSmallestUnitsString(this decimal d) => d.ToString("0.#");

        public static string GetFormattedTimeSpan(this TimeSpan ts) =>
            $"{ts.Hours} hour{(ts.Hours != 1 ? "s" : "")} " +
            $"{ts.Minutes} minute{(ts.Minutes != 1 ? "s" : "")} " +
            $"{ts.Seconds} second{(ts.Seconds != 1 ? "s" : "")} " +
            $"{ts.Milliseconds} millisecond{(ts.Milliseconds != 1 ? "s" : "")}";
    }

}
