using FiasGarImporter.Events;
using FiasGarImporter.Helpers;
using FiasGarImporter.Models;
using System.Text.RegularExpressions;

namespace FiasGarImporter
{
    public class GarImporter : IImporter
    {
        private const string dateGroupName = "date";
        private const string isDeltaGroupName = "isdelta";
        private const string listUrl = "https://fias.nalog.ru/DataArchive";

        private readonly string saveFolder;
        private readonly Func<IHttpClient> clientFactory;
        private readonly FileDownloader downloader;
        private static readonly Regex garUrlPattern = new("https://fias-file.nalog.ru/downloads/" +
            $@"(?<{dateGroupName}>\d{{4}}.\d{{2}}.\d{{2}})/" +
            $"(gar(?<{isDeltaGroupName}>_delta)?_xml.zip)",
            RegexOptions.Compiled);

        public event EventHandler<ProgressEventArgs>? Progress;

        public GarImporter(string tempFolder, Func<IHttpClient> clientFactory)
        {
            this.clientFactory = clientFactory;
            saveFolder = tempFolder ?? throw new ArgumentNullException(nameof(tempFolder));
            downloader = new FileDownloader(clientFactory);
            downloader.ProgressChanged += (size, total, pcnt) => Progress?.Invoke(this, new ProgressEventArgs(pcnt, pcnt >= 100));
        }

        public IEnumerable<AddressObject> GetFull()
        {
            IEnumerable<GarUrl> urls = GetUrlList();
            GarUrl lastFull = urls.Where(x => !x.IsDelta).OrderByDescending(x => x.Date).First();

            string fileName = Templates.GetFullFileName(lastFull.Date);
            fileName = Path.Combine(saveFolder, fileName);

            if (!File.Exists(fileName))
            {
                downloader.Download(lastFull.Url, fileName);
            }
            return default;
        }

        public async ValueTask<IEnumerable<AddressObject>> GetFullAsync()
        {
            IEnumerable<GarUrl> urls = await GetUrlListAsync();
            GarUrl lastFull = urls.Where(x => !x.IsDelta).OrderByDescending(x => x.Date).First();

            string fileName = Templates.GetFullFileName(lastFull.Date);
            fileName = Path.Combine(saveFolder, fileName);

            if (!File.Exists(fileName))
            {
                await downloader
                    .DownloadAsync(lastFull.Url, fileName)
                    .ConfigureAwait(false);
            }
            return default;
        }

        public IEnumerable<AddressObject> GetDiff(DateTime lastLoad)
        {
            IEnumerable<GarUrl> urls = GetUrlList();

            foreach (GarUrl? url in urls.Where(x => x.Date > lastLoad && x.IsDelta))
            {
                string fileName = Path.Combine(saveFolder, Templates.GetDiffFileName(url.Date));
                if (File.Exists(fileName))
                {
                    continue;
                }

                downloader.Download(url.Url, fileName);
            }
            return default;
        }

        public async ValueTask<IEnumerable<AddressObject>> GetDiffAsync(DateTime lastLoad)
        {
            IEnumerable<GarUrl> urls = await GetUrlListAsync();

            foreach (GarUrl? url in urls.Where(x => x.Date > lastLoad && x.IsDelta))
            {
                string fileName = Path.Combine(saveFolder, Templates.GetDiffFileName(url.Date));
                if (File.Exists(fileName))
                {
                    continue;
                }

                await downloader.DownloadAsync(url.Url, fileName)
                    .ConfigureAwait(false);
            }
            return default;
        }

        public async ValueTask<IEnumerable<GarUrl>> GetUrlListAsync()
        {
            using IHttpClient client = clientFactory();
            string content = await client.GetAsync<string>(listUrl);
            return GetUrlsFromPageContent(content);
        }

        public IEnumerable<GarUrl> GetUrlList()
        {
            using IHttpClient client = clientFactory();
            string content = client.Get<string>(listUrl);
            return GetUrlsFromPageContent(content);
        }

        private static IEnumerable<GarUrl> GetUrlsFromPageContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Array.Empty<GarUrl>();
            }

            MatchCollection matches = garUrlPattern.Matches(content);

            List<GarUrl> result = new();
            foreach (Match match in matches.Where(x => x.Success))
            {
                DateTime date = DateTime.Parse(match.Groups[dateGroupName].Value);
                bool isDelta = match.Groups[isDeltaGroupName].Length > 0;
                result.Add(new GarUrl(match.Value, date, isDelta));
            }
            return result.Distinct();
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
