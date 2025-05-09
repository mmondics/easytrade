using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BrokerService.Services.Preprocessing;

public class FraudDetectionClient : IFraudDetectionClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FraudDetectionClient> _logger;
    private readonly FraudPreprocessor _preprocessor;
    private readonly string _tritonUrl;

    public FraudDetectionClient(
        HttpClient httpClient,
        ILogger<FraudDetectionClient> logger,
        FraudPreprocessor preprocessor
    )
    {
        _httpClient    = httpClient;
        _logger        = logger;
        _preprocessor  = preprocessor;
        _tritonUrl     = Environment.GetEnvironmentVariable("TRITON_FRAUD_URL")
                         ?? throw new InvalidOperationException(
                                "Environment variable TRITON_FRAUD_URL is not set.");
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
        // 1) assemble raw feature vector
        var raw = new float[]
        {
            accountId,
            instrumentId,
            success   ? 1f : 0f,
            (float)quantity,
            (float)price,
            (float)totalValue,
            hour,
            day,
            isBuy     ? 1f : 0f
        };

        // 2) preprocess (standard scale)  
        var proc = _preprocessor.Transform(raw);

        // 3) build Triton JSON for the ONNX model input
        var payload = new
        {
            inputs = new[]
            {
                new
                {
                    name     = "input",
                    shape    = new[] { 1, proc.Length },
                    datatype = "FP32",
                    data     = new[] { proc }
                }
            }
        };

        var json = JsonSerializer.Serialize(payload);
        _logger.LogInformation("Posting to Triton at {Url}: {Payload}", _tritonUrl, json);

        using var content  = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync(_tritonUrl, content);
        response.EnsureSuccessStatusCode();

        // 4) parse Triton’s JSON response
        var doc = await response.Content.ReadFromJsonAsync<JsonDocument>();
        var score = doc!
            .RootElement
            .GetProperty("outputs")[0]
            .GetProperty("data")[0]
            .GetSingle();

        _logger.LogInformation("Received fraud score: {Score}", score);
        return score;
    }
}
