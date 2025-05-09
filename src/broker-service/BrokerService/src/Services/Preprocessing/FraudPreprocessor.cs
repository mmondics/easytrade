using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace BrokerService.Services.Preprocessing
{
    public class FraudPreprocessor
    {
        private readonly float[] _means, _scales;

        public FraudPreprocessor(IConfiguration config)
        {
            // Load the JSON file path from configuration
            var path = config["FraudPreprocessor:ParamsFile"]
                       ?? throw new InvalidOperationException(
                              "FraudPreprocessor:ParamsFile not configured");

            // Read & parse
            var json = File.ReadAllText(path);
            using var doc = JsonDocument.Parse(json);

            // Extract arrays
            _means  = doc.RootElement
                          .GetProperty("means")
                          .EnumerateArray()
                          .Select(e => e.GetSingle())
                          .ToArray();

            _scales = doc.RootElement
                          .GetProperty("scales")
                          .EnumerateArray()
                          .Select(e => e.GetSingle())
                          .ToArray();
        }

        public float[] Transform(float[] raw)
        {
            if (raw.Length != _means.Length)
                throw new ArgumentException(
                    $"Expected { _means.Length } features, got { raw.Length }");

            var proc = new float[raw.Length];
            for (int i = 0; i < raw.Length; i++)
                proc[i] = (raw[i] - _means[i]) / _scales[i];

            return proc;
        }
    }
}
