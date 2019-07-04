using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brook.MainWin.Common
{
    public static class PerfTracker
    {
        static Dictionary<string, Stopwatch> allPerfTrackers;

        static PerfTracker()
        {
            allPerfTrackers = new Dictionary<string, Stopwatch>();
        }

        public static void StartTracking(string perfMetricName)
        {
            Stopwatch existingStopWatch;
            if (allPerfTrackers.TryGetValue(perfMetricName, out existingStopWatch))
            {
                existingStopWatch.Reset();
            }
            else
            {
                existingStopWatch = new Stopwatch();
                allPerfTrackers.Add(perfMetricName, existingStopWatch);
            }
            existingStopWatch.Start();
        }

        public static TimeSpan StopTracking(string perfMetricName)
        {
            Stopwatch existingStopWatch;
            if (allPerfTrackers.TryGetValue(perfMetricName, out existingStopWatch))
            {
                existingStopWatch.Stop();
            }
            else
            {
                throw new ArgumentException($"No performance tracker found for {perfMetricName}");
            }
            return existingStopWatch.Elapsed;
        }
    }
}
