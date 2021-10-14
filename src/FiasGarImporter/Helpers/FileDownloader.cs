namespace FiasGarImporter.Helpers
{
    internal class FileDownloader : IDisposable
    {
        private bool disposedValue;
        private readonly HttpClient client;

        public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double progressPercentage);

        public event ProgressChangedHandler? ProgressChanged;

        public FileDownloader()
        {
            client = new HttpClient() { Timeout = TimeSpan.FromDays(1) };
        }

        public async Task DownloadAsync(string url, string filePath)
        {
            using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            await DownloadFileFromHttpResponseMessage(response, filePath);
        }

        public void Download(string url, string filePath)
        {
            using AutoResetEvent completedSignal = new(false);

            client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead)
                .ContinueWith(r =>
                {
                    DownloadFileFromHttpResponseMessage(r.Result, filePath)
                        .ContinueWith(x =>
                        {
                            r.Result.Dispose();
                            completedSignal.Set();
                        });
                });

            completedSignal.WaitOne();
        }

        private async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response, string filePath)
        {
            response.EnsureSuccessStatusCode();

            long? totalBytes = response.Content.Headers.ContentLength;

            using Stream contentStream = await response.Content.ReadAsStreamAsync();
            await ProcessContentStream(totalBytes, contentStream, filePath);
        }

        private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream, string filePath)
        {
            long totalBytesRead = 0L;
            long readCount = 0L;
            byte[] buffer = new byte[8192];
            bool isMoreToRead = true;

            using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
            do
            {
                int bytesRead = await contentStream.ReadAsync(buffer);
                if (bytesRead == 0)
                {
                    isMoreToRead = false;
                    TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                    continue;
                }

                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));

                totalBytesRead += bytesRead;
                readCount += 1;

                if (readCount % 100 == 0)
                {
                    TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                }
            }
            while (isMoreToRead);
        }

        private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead)
        {
            if (ProgressChanged == null)
            {
                return;
            }

            double progressPercentage = 0f;
            if (totalDownloadSize.HasValue)
            {
                progressPercentage = Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 2);
            }

            ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage);
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
    }
}
