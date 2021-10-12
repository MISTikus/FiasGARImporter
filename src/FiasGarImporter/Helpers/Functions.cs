namespace FiasGarImporter.Helpers
{
    internal static class Functions
    {
        internal static DateTime GetLastDayOfWeek(params int[] daysOfWeekToSearch)
        {
            DateTime result = DateTime.Today;
            int dow = GetDayOfWeekNum(result);

            while (!daysOfWeekToSearch.Contains(dow))
            {
                result = result.AddDays(-1);
                dow = GetDayOfWeekNum(result);
            }
            return result;
        }

        private static int GetDayOfWeekNum(DateTime result)
        {
            int dow = (int)result.DayOfWeek;
            dow = dow == 0 ? 7 : dow;
            return dow;
        }
    }
}
