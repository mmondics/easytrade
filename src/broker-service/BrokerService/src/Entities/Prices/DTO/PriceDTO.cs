using System.Text.Json.Serialization;
using EasyTrade.BrokerService.Entities.Trades.Controllers;

namespace EasyTrade.BrokerService.Entities.Prices.DTO;

public class PriceDTO
{
    [JsonPropertyName("instrumentId")]
    public int InstrumentId { get; set; }

    [JsonPropertyName("timestamp")]
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime? Timestamp { get; set; }

    [JsonPropertyName("open")]
    public decimal Open { get; set; }

    [JsonPropertyName("close")]
    public decimal Close { get; set; }

    [JsonPropertyName("low")]
    public decimal Low { get; set; }

    [JsonPropertyName("high")]
    public decimal High { get; set; }

    public PriceDTO() { }

    public PriceDTO(Price price)
    {
        InstrumentId = price.InstrumentId;
        Timestamp = price.Timestamp;
        Open = price.Open;
        Close = price.Close;
        Low = price.Low;
        High = price.High;
    }

    public Price ToEntity() =>
        new Price(InstrumentId, Timestamp ?? DateTime.MinValue, Open, High, Low, Close);
}
