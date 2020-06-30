using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBot.Business.Helper
{
    public static class Mapper
    {
        static private Dictionary<string, DayOfWeek> dayOfWeeks = new Dictionary<string, DayOfWeek>
        {
            { "Mo", DayOfWeek.Monday },
            { "Di", DayOfWeek.Tuesday },
            { "Mi", DayOfWeek.Wednesday },
            { "Do", DayOfWeek.Thursday },
            { "Fr", DayOfWeek.Friday },
            { "Sa", DayOfWeek.Saturday },
            { "So", DayOfWeek.Sunday },
        };

        public static DayOfWeek MapWeekday(string day)
        {
            DayOfWeek result = DayOfWeek.Sunday;

            dayOfWeeks.TryGetValue(day.ToLower(), out result);

            return result;
        }

        public static string MapWeekday(DayOfWeek day)
        {
            return dayOfWeeks.FirstOrDefault(x => x.Value == day).Key;
        }
    }
}
