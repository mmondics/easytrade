using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

public class FraudDetectionClient : IFraudDetectionClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FraudDetectionClient> _logger;
    private readonly string _tritonUrl;

    public FraudDetectionClient(HttpClient httpClient, ILogger<FraudDetectionClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _tritonUrl = Environment.GetEnvironmentVariable("TRITON_FRAUD_URL")
            ?? throw new InvalidOperationException("Environment variable TRITON_FRAUD_URL is not set.");
    }

    public async Task<float> ScoreTradeAsync(
        int accountId,
        int instrumentId,
        bool success,
        double quantity,
        double price,
        double totalValue,
        int hour,
        int day,
        bool isBuy
    )
    {
        var input = new
        {
            inputs = new[]
            {
                new
                {
                    name = "raw_input",
                    shape = new[] { 1, 9 },
                    datatype = "FP32",
                    data = new object[]
                    {
                        (float)accountId,
                        (float)instrumentId,
                        success ? 1f : 0f,
                        (float)quantity,
                        (float)price,
                        (float)totalValue,
                        (float)hour,
                        (float)day,
                        isBuy ? 1f : 0f
                    }
                }
            }
        };

        var requestJson = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");
        var payloadJson = JsonSerializer.Serialize(input);

        _logger.LogInformation("Sending payload to Triton at {url}: {json}", _tritonUrl, payloadJson);

        var response = await _httpClient.PostAsync(_tritonUrl, requestJson);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("outputs")[0].GetProperty("data")[0].GetSingle();
    }
}
