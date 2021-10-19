namespace FiasGarImporter.Helpers
{
    internal static class Templates
    {
        public const string FullFileName = "gar_xml.zip";
        public const string DiffFileName = "gar_delta_xml.zip";
        public static readonly int[] UpdateDayNums = new[] { 2, 5 };

        public static string GetFullFileName(DateTime date)
        {
            return $"gar_xml_{date:yyyy-MM-dd}.zip";
        }

        public static string GetDiffFileName(DateTime date)
        {
            return $"gar_delta_xml_{date:yyyy-MM-dd}.zip";
        }
    }
}
