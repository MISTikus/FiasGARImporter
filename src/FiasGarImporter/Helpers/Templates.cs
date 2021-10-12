namespace FiasGarImporter.Helpers
{
    internal static class Templates
    {
        public const string FullFileName = "gar_xml.zip";
        public const string DiffFileName = "gar_delta_xml.zip";

        public static string CurrentDateFullFileName => $"gar_xml_{DateTime.Today:yyyy-MM-dd}.zip";
        public static string CurrentDateDiffFileName => $"gar_delta_xml_{DateTime.Today:yyyy-MM-dd}.zip";

        public static string DownloadLastFullUrl
            => $"https://fias-file.nalog.ru/downloads/{Functions.GetLastDayOfWeek(2, 5):yyyy.MM.dd}/{FullFileName}";
        public static string DownloadLastDiffUrl
            => $"https://fias-file.nalog.ru/downloads/{Functions.GetLastDayOfWeek(2, 5):yyyy.MM.dd}/{DiffFileName}";
    }
}
