namespace FiasGarImporter.Helpers
{
    internal static class Functions
    {
        internal static DateTime GetLastDayOfWeek(params int[] daysOfWeekToSearch)
        {
            return GetLastDayOfWeek(DateTime.Today, daysOfWeekToSearch);
        }

        internal static DateTime GetLastDayOfWeek(this DateTime fromDate, params int[] daysOfWeekToSearch)
        {
            DateTime result = fromDate;
            int dow = GetDayOfWeekNum(result);

            while (!daysOfWeekToSearch.Contains(dow))
            {
                result = result.AddDays(-1);
                dow = GetDayOfWeekNum(result);
            }
            return result;
        }

        public static int GetDayOfWeekNum(this DateTime result)
        {
            int dow = (int)result.DayOfWeek;
            dow = dow == 0 ? 7 : dow;
            return dow;
        }
    }
}
