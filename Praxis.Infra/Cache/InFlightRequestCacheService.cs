using System.Collections.Concurrent;

namespace Praxis.Infra.Cache
{
    public class InFlightRequestCacheService
    {
        private readonly ConcurrentDictionary<string, Task<object>> _tasks = new();

        public Task<T> GetOrAddInFlightCache<T>(string key, Func<Task<T>> factory)
        {
            var task = _tasks.GetOrAdd(key, _ => WrapInFlightCache(factory, key));
            return task.ContinueWith(t => (T)t.Result);
        }

        private async Task<object> WrapInFlightCache<T>(Func<Task<T>> factory, string key)
        {
            try
            {
                return await factory();
            }
            finally
            {
                _tasks.TryRemove(key, out _);
            }
        }
    }
}
