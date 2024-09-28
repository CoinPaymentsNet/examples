using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsolidationApp.Helpers
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
        public static void AddInfo(string information, InfoType? type = InfoType.Success)
        {
            if (type == InfoType.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(information);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(information);
        }
    }

    public enum InfoType
    {
        Error = 0,
        Success = 1
    }
    
}
