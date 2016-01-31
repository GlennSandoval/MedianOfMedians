using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        public T NaiveMedian<T>(List<T> values)
        {
            values.Sort();
            return values[(int)(values.Count / 2)];
        }

        [TestMethod]
        public void TestMethod1()
        {
            var values = new List<int>();
            var rand = new Random();
            int value;

            while (values.Count < 1000)
            {
                value = rand.Next(1, 100000);

                if (!values.Contains(value))
                {
                    values.Add(value);
                }
            }


            Stopwatch sw = new Stopwatch();
            sw.Start();
            var result = Medians.Median.FindMedian<int>(values, (a, b) => { return a.CompareTo(b); });
            sw.Stop();
            Debug.WriteLine("Time {0}  Result {1}", sw.Elapsed, result.ToString());

            sw.Restart();
            result = NaiveMedian<int>(values);
            sw.Stop();
            Debug.WriteLine("Time {0}  Result {1}", sw.Elapsed, result.ToString());
        }
    }
}
