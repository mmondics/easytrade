using System.Text.Json.Serialization;

namespace EasyTrade.BrokerService.Entities.Prices.DTO
{
    public class PricesResultDto
    {
        [JsonPropertyName("results")]
        public List<PriceDTO> Results { get; set; } = new();
    }
}
