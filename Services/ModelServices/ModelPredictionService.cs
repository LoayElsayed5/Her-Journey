using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ServicesAbstraction.ModelAbstraction;
using Shared.DTos.MlDTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.ModelServices
{
    public class ModelPredictionService : IModelPredictionService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ModelPredictionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<PredictionResponseDto> PredictAsync(PredictionRequestDto request)
        {
            var endpoint = _configuration["ModelApi:PredictEndpoint"] ?? "predict";

            using var response = await _httpClient.PostAsJsonAsync(endpoint, request);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Model API failed. StatusCode: {(int)response.StatusCode}, Body: {responseBody}");

            var result = JsonSerializer.Deserialize<PredictionResponseDto>(
                responseBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result is null)
                throw new Exception("Invalid response returned from model API.");

            return result;
        }
    }
}
