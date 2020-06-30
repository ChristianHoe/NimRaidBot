//using EventBot.Business.Commands;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EventBot.Business.Helper
//{
//    public static class ExtractTimes
//    {
//        public static IEnumerable<Time> Extract(string[] text, int groupId, IEnumerable<DataAccess.Queries.Eventtype> eventTypes)
//        {
//            var numberOfParts = text.Count();
//            if (numberOfParts < 2 || 3 < numberOfParts)
//            {
//                return null;
//            }

//            var offsets = ExtractTimes.ExtractWeekdayOffsets(text[0]).SkipWhile(x => x == null).ToArray();
//            var hours = ExtractTimes.ExtractHours(text[1]).SkipWhile(x => x == null).ToArray();
//            var eventIds = ExtractTimes.ExtractEventtypes(numberOfParts == 3 ? text[2] : string.Empty, eventTypes).ToArray();

//            int numberOfDays = offsets.Count();
//            if (numberOfDays == 0)
//                return Enumerable.Empty<Time>();

//            int numberOfHours = hours.Count();
//            if (numberOfHours == 0)
//                return Enumerable.Empty<Time>();

//            int numberOfEventIds = eventIds.Count();
//            if (numberOfEventIds == 0)
//                return Enumerable.Empty<Time>();

//            int value = 0;
//            for (int i = 0; i < numberOfEventIds; i++)
//            {
//                value += (1 << eventIds[i].Value);
//            }

//            var today = DateTime.Now.Date;

//            Time[] times = new Time[numberOfDays];
//            for (int i = 0; i < numberOfDays; i++)
//            {
//                times[i] = new Time() { Date = today.AddDays(offsets[i].Value), GroupId = groupId };
//                var current = times[i];

//                foreach (var hour in hours)
//                {
//                    current.Hours[hour.Value] = value;
//                }
//            }

//            return times;
//        }

//        private static int?[] ExtractWeekdayOffsets(string text)
//        {
//            if (string.IsNullOrWhiteSpace(text))
//                return new int?[0];

//            var weekdays = text.Split(',');

//            int numberOfDays = weekdays.Count();

//            var today = DateTime.Now.Date;

//            int?[] offsets = new int?[numberOfDays];
//            for (int i = 0; i < numberOfDays; i++)
//            {
//                offsets[i] = CalculateWeekdayOffset(TranslateToDayOfWeek(weekdays[i]), today.DayOfWeek);
//            }

//            return offsets;
//        }

//        private static int?[] ExtractHours(string text)
//        {
//            if (string.IsNullOrWhiteSpace(text))
//                return new int?[0];

//            var hoursAsString = text.Split(',');

//            int numberOfHours = hoursAsString.Count();

//            int?[] hours = new int?[numberOfHours];
//            for (int i = 0; i < numberOfHours; i++)
//            {
//                int hourAsInt;
//                if (int.TryParse(hoursAsString[i], out hourAsInt))
//                {
//                    hours[i] = (0 <= hourAsInt && hourAsInt <= 23) ? hourAsInt : (int?)null;
//                }
//            }

//            return hours;
//        }

//        private static int?[] ExtractEventtypes(string text, IEnumerable<DataAccess.Queries.Eventtype> eventTypes)
//        {
//            if (string.IsNullOrWhiteSpace(text))
//            {
//                if (eventTypes.Count() == 1)
//                    return new int?[] { eventTypes.First().Number };

//                return new int?[0];
//            }

//            var eventtypesAsString = text.Split(',');

//            var result = new List<int?>();
//            foreach(var eventtype in eventtypesAsString)
//            {
//                var found = eventTypes.FirstOrDefault(x => x.Name.ToLower() == eventtype.ToLower());
//                if (found == null)
//                    result.Add(null);
//                else
//                    result.Add(found.Number);
//            }

//            return result.ToArray();
//        }

//        public static DayOfWeek? TranslateToDayOfWeek(string day)
//        {
//            switch (day.ToLower())
//            {
//                case "mo":
//                    return DayOfWeek.Monday;
//                case "di":
//                    return DayOfWeek.Tuesday;
//                case "mi":
//                    return DayOfWeek.Wednesday;
//                case "do":
//                    return DayOfWeek.Thursday;
//                case "fr":
//                    return DayOfWeek.Friday;
//                case "sa":
//                    return DayOfWeek.Saturday;
//                case "so":
//                    return DayOfWeek.Sunday;
//                default:
//                    return null;
//            }
//        }

//        public static int? CalculateWeekdayOffset(DayOfWeek? target, DayOfWeek now)
//        {
//            if (!target.HasValue)
//                return null;

//            var offset = (int)target - (int)now;
//            if (offset < 0)
//                offset += 7;

//            return offset;
//        }

//    }
//}
