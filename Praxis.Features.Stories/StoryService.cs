using Praxis.Infra;
using Praxis.Infra.Cache;
using System.Diagnostics;

namespace Praxis.Features.Stories
{
    public class StoryService : IStoryService
    {
        private readonly HttpHackerNewsClient _client;
        private readonly RedisCacheService _cache;
        private readonly InFlightRequestCacheService _inFlightCache;

        public StoryService(HttpHackerNewsClient client,
        RedisCacheService cache,
        InFlightRequestCacheService inFlightCache)
        {
            _client = client;
            _cache = cache;
            _inFlightCache = inFlightCache;

        }
        public Task<List<StoryResponse>> GetBestStories(int n)
        {
            var key = $"stories_{n}";

            return _inFlightCache.GetOrAddInFlightCache(key, () => GetBestStoriesInternal(n, key));
        }

        private async Task<List<StoryResponse>> GetBestStoriesInternal(int n, string key)
        {
            try
            {
                // 1. Cache
                var cached = await _cache.GetCacheAsync<List<StoryResponse>>(key);

                if (cached != null)
                {
                    return cached;
                }


                // 2. IDs
                var ids = await _client.GetBestStoryIds();

                if (ids == null || ids.Count == 0)
                {
                    return new List<StoryResponse>();
                }

                // 3. Fetch stories
                var tasks = ids.Take(100).Select(async id =>
                {
                    try
                    {
                        return await _client.GetStory(id);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                });

                var items = await Task.WhenAll(tasks);

                var result = items
                    .Where(x => x != null)
                    .OrderByDescending(x => x.Score)
                    .Take(n)
                    .Select(x => new StoryResponse(
                        x.Title,
                        x.Url,
                        x.By,
                        DateTimeOffset.FromUnixTimeSeconds(x.Time).DateTime,
                        x.Score,
                        x.Descendants))
                    .ToList();

                // 4. Cache
                await _cache.SetCacheAsync(key, result, TimeSpan.FromMinutes(5));

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
