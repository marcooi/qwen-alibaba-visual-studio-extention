using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QwenAlibabaCodingPlan
{
    public class QwenApiClient
    {
        private readonly HttpClient _httpClient;
        private string _apiKey;
        private string _apiUrl = "https://coding-intl.dashscope.aliyuncs.com/apps/anthropic";
        private string _model = "qwen3.5-plus";

        public QwenApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public void Configure(string apiKey, string apiUrl = null, string model = null)
        {
            _apiKey = apiKey;
            if (!string.IsNullOrEmpty(apiUrl))
                _apiUrl = apiUrl;
            if (!string.IsNullOrEmpty(model))
                _model = model;
        }

        public async Task<string> SendChatMessageAsync(string message, List<ChatMessage> conversationHistory = null)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return "Error: API key not configured. Please set your Qwen API key in Tools > Qwen Settings.";
            }

            var messages = new List<Dictionary<string, string>>();

            if (conversationHistory != null)
            {
                foreach (var msg in conversationHistory)
                {
                    messages.Add(new Dictionary<string, string>
                    {
                        { "role", msg.Role },
                        { "content", msg.Content }
                    });
                }
            }

            messages.Add(new Dictionary<string, string>
            {
                { "role", "user" },
                { "content", message }
            });

            var requestBody = new
            {
                model = _model,
                input = new
                {
                    messages = messages
                },
                parameters = new
                {
                    result_format = "message"
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _httpClient.DefaultRequestHeaders.Add("X-DashScope-Async", "disable");

            try
            {
                var response = await _httpClient.PostAsync(_apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"API Error ({response.StatusCode}): {responseContent}";
                }

                var responseObj = JsonConvert.DeserializeObject<QwenResponse>(responseContent);
                return responseObj?.Output?.Choices?[0]?.Message?.Content ?? "No response received.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> GetCodeCompletionAsync(string code, string language)
        {
            var prompt = $"Complete the following {language} code. Only provide the completion, no explanations:\n\n{code}";
            return await SendChatMessageAsync(prompt);
        }

        public async Task<string> AnalyzeCodeAsync(string code)
        {
            var prompt = $"Analyze the following code and provide:\n1. What it does\n2. Potential issues or bugs\n3. Suggestions for improvement:\n\n```{code}```";
            return await SendChatMessageAsync(prompt);
        }

        public async Task<string> RefactorCodeAsync(string code, string goal)
        {
            var prompt = $"Refactor the following code to {goal}. Provide only the refactored code:\n\n```{code}```";
            return await SendChatMessageAsync(prompt);
        }

        public async Task<string> GenerateCodeAsync(string prompt, string language)
        {
            var fullPrompt = $"Generate {language} code for the following request. Provide only the code, no explanations:\n\n{prompt}";
            return await SendChatMessageAsync(fullPrompt);
        }
    }

    public class ChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }

        public ChatMessage(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }

    public class QwenResponse
    {
        [JsonProperty("output")]
        public OutputData Output { get; set; }

        [JsonProperty("usage")]
        public UsageData Usage { get; set; }

        [JsonProperty("request_id")]
        public string RequestId { get; set; }
    }

    public class OutputData
    {
        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        [JsonProperty("finish_reason")]
        public string FinishReason { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }
    }

    public class Message
    {
        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class UsageData
    {
        [JsonProperty("input_tokens")]
        public int InputTokens { get; set; }

        [JsonProperty("output_tokens")]
        public int OutputTokens { get; set; }
    }
}