using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyTrade.BrokerService.Helpers
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Attempt to parse the string as a DateTime in the expected format
            if (DateTime.TryParseExact(reader.GetString(), DateFormat, null, System.Globalization.DateTimeStyles.None, out var dateTime))
            {
                return dateTime;
            }
            else
            {
                // Handle invalid date format by throwing exception or returning default value
                throw new JsonException($"Unable to convert '{reader.GetString()}' to a DateTime.");
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Convert DateTime to string format before serialization
            writer.WriteStringValue(value.ToString(DateFormat));
        }
    }
}
