namespace CoinPayments.IntegrationTest.Models;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class StringToIntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string stringValue = reader.GetString();
            if (int.TryParse(stringValue, out int result))
            {
                return result;
            }
            return 0; // Default value if parsing fails
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }
        
        return 0; // Default value for other cases
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}