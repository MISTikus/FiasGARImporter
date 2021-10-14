using FiasGarImporter.Events;
using FiasGarImporter.Helpers;
using FiasGarImporter.Models;

namespace FiasGarImporter
{
    public class GarImporter : IImporter
    {
        private readonly string saveFolder;
        private readonly FileDownloader downloader;

        public event EventHandler<ProgressEventArgs>? Progress;

        public GarImporter(string tempFolder)
        {
            saveFolder = tempFolder ?? throw new ArgumentNullException(nameof(tempFolder));
            downloader = new FileDownloader();
            downloader.ProgressChanged += (size, total, pcnt) => Progress?.Invoke(this, new ProgressEventArgs(pcnt, pcnt >= 100));
        }

        public IEnumerable<AddressObject> GetFull()
        {
            downloader.DownloadAsync(Templates.DownloadLastFullUrl, Path.Combine(saveFolder, Templates.CurrentDateFullFileName))
                .GetAwaiter()
                .GetResult();
            return default;
        }

        public async ValueTask<IEnumerable<AddressObject>> GetFullAsync()
        {
            await downloader.DownloadAsync(Templates.DownloadLastFullUrl, Path.Combine(saveFolder, Templates.CurrentDateFullFileName))
                .ConfigureAwait(false);
            return default;
        }

        public IEnumerable<AddressObject> GetDiff(DateTime lastLoad)
        {
            IEnumerable<(DateTime fileDate, string url)>? urls = Templates.DownloadDiffUrls(lastLoad);
            foreach ((DateTime fileDate, string url) url in urls)
            {
                string? fileName = Path.Combine(saveFolder, Templates.GetDiffFileName(url.fileDate));
                if (File.Exists(fileName))
                {
                    continue;
                }

                downloader.Download(url.url, fileName);
            }
            return default;
        }

        public async ValueTask<IEnumerable<AddressObject>> GetDiffAsync(DateTime lastLoad)
        {
            IEnumerable<(DateTime fileDate, string url)>? urls = Templates.DownloadDiffUrls(lastLoad);
            foreach ((DateTime fileDate, string url) url in urls)
            {
                string? fileName = Path.Combine(saveFolder, Templates.GetDiffFileName(url.fileDate));
                if (File.Exists(fileName))
                {
                    continue;
                }

                await downloader.DownloadAsync(url.url, fileName)
                    .ConfigureAwait(false);
            }
            return default;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                downloader?.Dispose();
            }
        }
    }
}
