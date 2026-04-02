using Praxis.Infra.Models;
using System.Net.Http.Json;

namespace Praxis.Infra
{
    public class HttpHackerNewsClient
    {
        private readonly HttpClient _http;

        public HttpHackerNewsClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<int>> GetBestStoryIds()
        => await _http.GetFromJsonAsync<List<int>>("beststories.json");

        public async Task<HackerNewsItem> GetStory(int id)
            => await _http.GetFromJsonAsync<HackerNewsItem>($"item/{id}.json");
    }
}
