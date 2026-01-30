using System.Net.Http;
using System.Text;
using System.Text.Json;


namespace BookHub.ApiGateway
{
    public class GatewayHttpClient
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;


        public GatewayHttpClient(string baseUrl, HttpClient client)
        {
            _baseUrl = baseUrl;
            _client = client;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await _client.GetAsync(_baseUrl + url);
            response.EnsureSuccessStatusCode();


            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            )!;
        }

        public async Task<T> PostAsync<T, U>(string url, U postData)
        {
            var content = new StringContent(
            JsonSerializer.Serialize(postData),
            Encoding.UTF8,
            "application/json");


            var response = await _client.PostAsync(_baseUrl + url, content);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"POST {_baseUrl + url} failed with status {response.StatusCode}");


            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json) ?? throw new Exception("Result null");
        }

        public async Task<T> PutAsync<T, U>(string url, U putData)
        {
            var content = new StringContent(
            JsonSerializer.Serialize(putData),
            Encoding.UTF8,
            "application/json");


            var response = await _client.PutAsync(_baseUrl + url, content);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"PUT {_baseUrl + url} failed with status {response.StatusCode}");


            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json) ?? throw new Exception("Result null");
        }
    }
}