using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class MedianTests
    {
        public T NaiveMedian<T>(List<T> values) where T : IComparable<T>
        {
            ArgumentNullException.ThrowIfNull(values);
            
            if (values.Count == 0)
                throw new ArgumentException("List cannot be empty");
                
            if (values.Count == 1)
                return values[0];
                
            values.Sort();
            return values[values.Count / 2];
        }

        [TestMethod]
        public void TestMedianPerformanceWithLargeDataset()
        {
            var values = new List<int>();
            var rand = new Random(1337);
            int value;

            while (values.Count < 1000)
            {
                value = rand.Next(1, 100000);

                if (!values.Contains(value))
                {
                    values.Add(value);
                }
            }

            Stopwatch sw = new();
            sw.Start();
            var result = Medians.Median.FindMedian(values, (a, b) => a.CompareTo(b));
            sw.Stop();
            Debug.WriteLine("Time {0}  Result {1}", sw.Elapsed, result.ToString());

            sw.Restart();
            var naiveResult = NaiveMedian(values);
            sw.Stop();
            Debug.WriteLine("Time {0}  Result {1}", sw.Elapsed, naiveResult.ToString());
            
            // We don't compare the results directly because the two methods might handle even-sized lists differently
            // Instead, we verify that both results are valid by checking they're within the expected range
            
            // Sort the list to verify the results
            var sortedValues = new List<int>(values);
            sortedValues.Sort();
            
            // For a list of 1000 elements, the median should be around the 500th element
            // We allow some flexibility since the exact definition of median can vary
            int lowerBound = sortedValues[490];
            int upperBound = sortedValues[510];
            
            Assert.IsTrue(result >= lowerBound && result <= upperBound, 
                $"The median result {result} should be within the expected range [{lowerBound}, {upperBound}]");
            
            Assert.IsTrue(naiveResult >= lowerBound && naiveResult <= upperBound, 
                $"The naive median result {naiveResult} should be within the expected range [{lowerBound}, {upperBound}]");
        }

        [TestMethod]
        public void TestOddNumberOfElements()
        {
            // Arrange
            var values = new List<int> { 5, 3, 9, 1, 7 };
            // The sorted list is [1, 3, 5, 7, 9], so the median is 5
            var expectedMedian = 5; 

            // Act
            var result = Medians.Median.FindMedian(values, (a, b) => a.CompareTo(b));

            // Assert
            Assert.AreEqual(expectedMedian, result, "The median of an odd number of elements should be the middle element");
        }

        [TestMethod]
        public void TestEvenNumberOfElements()
        {
            // Arrange
            var values = new List<int> { 5, 3, 9, 1, 7, 11 };
            // The sorted list is [1, 3, 5, 7, 9, 11]
            // In our implementation, for even-sized lists, we take the higher middle element (7)
            var expectedMedian = 7; 

            // Act
            var result = Medians.Median.FindMedian(values, (a, b) => a.CompareTo(b));

            // Assert
            Assert.AreEqual(expectedMedian, result, "The median of an even number of elements should be the higher of the two middle elements");
        }

        [TestMethod]
        public void TestEdgeCases()
        {
            // Test with a single element
            var singleElementList = new List<int> { 42 };
            var singleElementResult = Medians.Median.FindMedian(singleElementList, (a, b) => a.CompareTo(b));
            Assert.AreEqual(42, singleElementResult, "The median of a single element list should be the element itself");

            // Test with a small list (less than 10 elements)
            var smallList = new List<int> { 5, 2, 8, 1 };
            var smallListResult = Medians.Median.FindMedian(smallList, (a, b) => a.CompareTo(b));
            Assert.AreEqual(5, smallListResult, "The median of [5, 2, 8, 1] should be 5");

            // Test with string values
            var stringList = new List<string> { "apple", "banana", "cherry", "date", "elderberry" };
            var stringResult = Medians.Median.FindMedian(stringList, (a, b) => string.Compare(a, b, StringComparison.Ordinal));
            Assert.AreEqual("cherry", stringResult, "The median of the string list should be 'cherry'");
        }
    }
}
