using System.Text.Json;

namespace FiasGarImporter.Helpers
{
    internal class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient client;
        private bool disposedValue;

        public HttpClientWrapper()
        {
            client = new HttpClient { Timeout = TimeSpan.FromDays(1) };
        }

        public T? Get<T>(string url, HttpCompletionOption option = default)
        {
            using AutoResetEvent completedSignal = new(false);

            string? content = null;
            Exception? e = null;
            GetAsync(url, option).AsTask()
                .ContinueWith(r =>
                {
                    if (r.IsFaulted)
                    {
                        e = r.Exception;
                        completedSignal.Set();
                        return;
                    }

                    r.Result.Content?.ReadAsStringAsync()
                        .ContinueWith(x =>
                        {
                            content = x.Result;
                            r.Result.Dispose();
                            completedSignal.Set();
                        });
                });

            completedSignal.WaitOne();
            if (e is not null)
            {
                throw e;
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return default;
            }

            return typeof(T) == typeof(string)
                ? (T)(object)content
                : JsonSerializer.Deserialize<T>(content);
        }

        public async ValueTask<T?> GetAsync<T>(string url, HttpCompletionOption option = default, CancellationToken cancellation = default)
        {
            HttpResponseMessage response = await GetAsync(url, option, cancellation);
            string? content = await response.Content.ReadAsStringAsync(cancellation);

            if (string.IsNullOrWhiteSpace(content))
            {
                return default;
            }

            return typeof(T) == typeof(string)
                ? (T)(object)content
                : JsonSerializer.Deserialize<T>(content);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask<HttpResponseMessage> GetAsync(string url, HttpCompletionOption option = HttpCompletionOption.ResponseContentRead, CancellationToken cancellation = default)
        {
            HttpResponseMessage response = await client.GetAsync(url, option, cancellation);
            response.EnsureSuccessStatusCode();
            return response;
        }
    }

    public interface IHttpClient : IDisposable
    {
        T Get<T>(string url, HttpCompletionOption option = default);
        ValueTask<T> GetAsync<T>(string url, HttpCompletionOption option = default, CancellationToken cancellation = default);
        ValueTask<HttpResponseMessage> GetAsync(string url, HttpCompletionOption option = default, CancellationToken cancellation = default);
    }
}
