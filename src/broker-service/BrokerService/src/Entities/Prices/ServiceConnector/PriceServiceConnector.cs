using System.Net; // For HttpStatusCode
using System.Text.Json; // For JsonSerializer and JsonSerializerOptions
using EasyTrade.BrokerService.Entities.Prices.DTO;
using EasyTrade.BrokerService.Helpers;
using EasyTrade.BrokerService.Entities.Trades.Controllers;

namespace EasyTrade.BrokerService.Entities.Prices.ServiceConnector
{
    public class PriceServiceConnector(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<PriceServiceConnector> logger
    ) : IPriceServiceConnector
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly ILogger _logger = logger;

        private string PriceServiceUrl => $"http://{_configuration[Constants.PricingService]}/";

        public async Task<IEnumerable<Price>> GetPricesByInstrumentId(int id, int count = 10)
        {
            _logger.LogInformation(
                "Fetching prices with instrument ID [{id}], count [{count}]",
                id,
                count
            );

            var endpoint = $"v1/prices/instrument/{id}?records={count}";
            using var client = GetHttpClient();
            using var response = await client.GetAsync(endpoint);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var rawJson = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Raw JSON (by instrument ID): {json}", rawJson);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new CustomDateTimeConverter());

                var pricesResult = JsonSerializer.Deserialize<PricesResultDto>(rawJson, options);
                var prices = (pricesResult?.Results ?? []).Select(dto => dto.ToEntity()).ToList();

                _logger.LogWarning("Deserialized prices: {prices}", prices.ToJson());
                return prices;
            }

            _logger.LogError("Fetch failed with status code [{statusCode}]", response.StatusCode);
            return Array.Empty<Price>();
        }


        public async Task<IEnumerable<Price>> GetLatestPrices()
        {
            _logger.LogInformation("Fetching latest prices");

            const string endpoint = "v1/prices/latest";
            using var client = GetHttpClient();
            using var response = await client.GetAsync(endpoint);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var rawJson = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Raw JSON (latest): {json}", rawJson);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new CustomDateTimeConverter());

                var pricesResult = JsonSerializer.Deserialize<PricesResultDto>(rawJson, options);
                var prices = (pricesResult?.Results ?? []).Select(dto => dto.ToEntity()).ToList();

                _logger.LogWarning("Deserialized prices: {prices}", prices.ToJson());
                return prices;
            }

            _logger.LogError("Fetch failed with status code [{statusCode}]", response.StatusCode);
            return Array.Empty<Price>();
        }

        public async Task<Price?> GetLastPriceByInstrumentId(int id)
        {
            var priceArray = await GetPricesByInstrumentId(id, 1);
            return priceArray.FirstOrDefault();
        }

        private HttpClient GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(PriceServiceUrl);
            return client;
        }
    }
}
