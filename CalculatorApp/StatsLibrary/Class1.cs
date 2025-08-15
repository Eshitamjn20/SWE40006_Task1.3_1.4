using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StatsLibrary
{
    public static class Stats
    {
        public static double Mean(IEnumerable<double> numbers) => numbers.Average();

        public static double Median(IEnumerable<double> numbers)
        {
            var sorted = numbers.OrderBy(v => v).ToArray();
            int count = sorted.Length;
            if (count == 0) return double.NaN;
            return (count % 2 == 1) ? sorted[count / 2]
                                    : (sorted[(count / 2) - 1] + sorted[count / 2]) / 2.0;
        }

        public static double StdDev(IEnumerable<double> numbers)
        {
            var arr = numbers.ToArray();
            if (arr.Length == 0) return double.NaN;
            double m = arr.Average();
            return Math.Sqrt(arr.Select(v => (v - m) * (v - m)).Average());
        }
    }
}

