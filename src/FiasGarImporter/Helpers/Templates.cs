namespace FiasGarImporter.Helpers
{
    internal static class Templates
    {
        public const string FullFileName = "gar_xml.zip";
        public const string DiffFileName = "gar_delta_xml.zip";
        public static readonly int[] updateDayNums = new[] { 2, 5 };

        public static string CurrentDateFullFileName => $"gar_xml_{DateTime.Today:yyyy-MM-dd}.zip";

        public static string DownloadLastFullUrl
            => $"https://fias-file.nalog.ru/downloads/{Functions.GetLastDayOfWeek(updateDayNums):yyyy.MM.dd}/{FullFileName}";
        public static string DownloadLastDiffUrl
            => $"https://fias-file.nalog.ru/downloads/{Functions.GetLastDayOfWeek(updateDayNums):yyyy.MM.dd}/{DiffFileName}";

        public static string GetDiffFileName(DateTime date)
        {
            return $"gar_delta_xml_{date:yyyy-MM-dd}.zip";
        }

        public static string DownloadDiffUrl(DateTime date)
        {
            return $"https://fias-file.nalog.ru/downloads/{Functions.GetLastDayOfWeek(date, updateDayNums):yyyy.MM.dd}/{DiffFileName}";
        }

        public static IEnumerable<(DateTime fileDate, string url)> DownloadDiffUrls(DateTime fromDate, DateTime? toDate = null)
        {
            toDate ??= DateTime.Today;
            if (!updateDayNums.Contains(fromDate.GetDayOfWeekNum()))
            {
                fromDate = fromDate.AddDays(fromDate.GetNextUpdateDayNum() + 1);
            }
            while (fromDate <= toDate)
            {
                DateTime last = Functions.GetLastDayOfWeek(fromDate, updateDayNums);
                yield return (last, $"https://fias-file.nalog.ru/downloads/{last:yyyy.MM.dd}/{DiffFileName}");
                fromDate = fromDate.AddDays(fromDate.GetNextUpdateDayNum());
            }
        }

        private static double GetNextUpdateDayNum(this DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Sunday => 3,
                DayOfWeek.Monday => 2,
                DayOfWeek.Tuesday => 1,
                DayOfWeek.Wednesday => 3,
                DayOfWeek.Thursday => 2,
                DayOfWeek.Friday => 1,
                DayOfWeek.Saturday => 4,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
