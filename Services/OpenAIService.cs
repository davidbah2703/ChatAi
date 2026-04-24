using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ChatAI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace ChatAI.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _http;
        private readonly OpenAIOptions _options;
        private readonly ILogger<OpenAIService> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public OpenAIService(HttpClient http, IOptions<OpenAIOptions> options, ILogger<OpenAIService> logger)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrWhiteSpace(_options.ApiKey))
                throw new InvalidOperationException("OpenAI API key is missing.");

            _http.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);

            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => IsTransient(r.StatusCode))
                .Or<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    retryCount: _options.MaxRetries,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)) + TimeSpan.FromMilliseconds(new Random().Next(0, 250)),
                    onRetry: (outcome, delay, attempt, _) =>
                    {
                        var reason = outcome.Exception?.Message ?? $"HTTP {(int)outcome.Result.StatusCode}";
                        _logger.LogWarning("OpenAI call failed (attempt {Attempt}/{Max}): {Reason}. Retrying in {Delay}s...", attempt, _options.MaxRetries, reason, delay.TotalSeconds);
                    });
        }

        public async Task<string> SendMessageAsync(IEnumerable<Message> history, CancellationToken cancellationToken = default)
        {
            if (history == null) throw new ArgumentNullException(nameof(history));

            var payload = new ChatCompletionRequest
            {
                Model = _options.Model,
                Temperature = _options.Temperature,
                Messages = history.Select(m => new ChatMessageDto { Role = m.Role, Content = m.Content }).ToList()
            };

            _logger.LogInformation("Sending {Count} messages to model {Model}", payload.Messages.Count, payload.Model);

            var response = await _retryPolicy.ExecuteAsync(async ct =>
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "chat/completions") { Content = JsonContent.Create(payload) };
                return await _http.SendAsync(req, ct).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("OpenAI request failed: {Status} - {Body}", response.StatusCode, errorBody);
                throw new HttpRequestException($"OpenAI API returned {(int)response.StatusCode}: {errorBody}");
            }

            var result = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
            var reply = result?.Choices?.FirstOrDefault()?.Message?.Content;
            if (string.IsNullOrEmpty(reply)) throw new InvalidOperationException("OpenAI returned an empty response.");
            return reply;
        }

        private static bool IsTransient(HttpStatusCode status)
        {
            return status == HttpStatusCode.TooManyRequests || status == HttpStatusCode.RequestTimeout || (int)status >= 500;
        }

        private class ChatCompletionRequest
        {
            [JsonPropertyName("model")] public string Model { get; set; } = "gpt-4o-mini";
            [JsonPropertyName("messages")] public List<ChatMessageDto> Messages { get; set; } = new();
            [JsonPropertyName("temperature")] public double Temperature { get; set; } = 0.7;
        }
        private class ChatMessageDto
        {
            [JsonPropertyName("role")] public string Role { get; set; } = "user";
            [JsonPropertyName("content")] public string Content { get; set; } = string.Empty;
        }
        private class ChatCompletionResponse
        {
            [JsonPropertyName("choices")] public List<Choice>? Choices { get; set; }
        }
        private class Choice
        {
            [JsonPropertyName("message")] public ChatMessageDto? Message { get; set; }
        }
    }
}