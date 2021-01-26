using System;
using System.Collections.Generic;
using System.Text;

namespace IntroProject
{
    public class StatisticsValues
    {
        public static IList<Statistics> statisticsvalues = new List<Statistics>();

        public static void AddStats(Statistics stats) => statisticsvalues.Add(stats);
        public static void ClearStats() => statisticsvalues.Clear();
    }
}
