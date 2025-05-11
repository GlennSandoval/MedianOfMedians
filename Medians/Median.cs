using System;
using System.Collections.Generic;
using System.Linq;

namespace Medians
{
    public static class Median
    {
        public static T FindMedian<T>(IEnumerable<T> list, Comparison<T> compare)
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(compare);
            
            var values = new List<T>(list);
            if (values.Count == 0)
                throw new ArgumentException("Must provide at least 1 item");

            // For a single element, return it directly
            if (values.Count == 1)
                return values[0];

            var count = values.Count;

            // For odd number of elements, the median is at index count/2
            // For even number of elements, we take the higher of the two middle elements
            var position = count / 2 + 1; // +1 because Select uses 1-based positions
            
            return Select(values, position, compare);
        }

        private static T Select<T>(IEnumerable<T> list, int position, Comparison<T> compare)
        {
            var values = new List<T>(list);
            
            // Handle small lists directly by sorting
            if (values.Count < 10)
            {
                var l = new List<T>(values);
                l.Sort(compare);
                return l[position - 1]; // position is 1-based
            }

            var s = new List<List<T>>();

            var partitions = values.Count / 5;
            if (partitions == 0) partitions = 1; // Ensure at least one partition

            var wrapper = new List<T>(values);

            // Create partitions of size 5
            for (var i = 0; i < partitions; i++)
            {
                int startIndex = i * 5;
                int count = Math.Min(5, wrapper.Count - startIndex);
                if (count > 0)
                {
                    s.Add(new List<T>(wrapper.GetRange(startIndex, count)));
                }
            }

            // Find median of each partition
            var medians = s.Select(sl => Select(sl, (sl.Count + 1) / 2, compare)).ToList();

            // Find median of medians
            var medianOfMedians = Select(medians, (medians.Count + 1) / 2, compare);

            var l1 = new List<T>();
            var l3 = new List<T>();

            // Partition elements around the median of medians
            foreach (var d in values)
                if (compare(d, medianOfMedians) < 0)
                    l1.Add(d);
                else
                    l3.Add(d);

            // Recursively find the kth element
            if (position <= l1.Count)
                return Select(l1, position, compare);
            else if (position > l1.Count + 1)
                return Select(l3, position - l1.Count - 1, compare);
            else
                return medianOfMedians;
        }

        private static int FastSelect<T>(T[] list, int lo, int hi, int k) where T : IComparable
        {
            var n = hi - lo;
            if (n < 2)
                return lo;

            var pivot = list[lo + k * 7919 % n]; // Pick a random pivot

            // Triage list to [<pivot][=pivot][>pivot]
            int nLess = 0, nSame = 0, nMore = 0;
            var lo3 = lo;
            var hi3 = hi;
            while (lo3 < hi3)
            {
                var e = list[lo3];
                var cmp = e.CompareTo(pivot);
                if (cmp < 0)
                {
                    nLess++;
                    lo3++;
                }
                else if (cmp > 0)
                {
                    list.Swap(lo3, --hi3);
                    if (nSame > 0)
                        list.Swap(hi3, hi3 + nSame);
                    nMore++;
                }
                else
                {
                    nSame++;
                    list.Swap(lo3, --hi3);
                }
            }

            if (k >= n - nMore)
                return FastSelect(list, hi - nMore, hi, k - nLess - nSame);
            else if (k < nLess)
                return FastSelect(list, lo, lo + nLess, k);
            return lo + k;
        }

        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            ArgumentNullException.ThrowIfNull(list);
            
            (list[indexB], list[indexA]) = (list[indexA], list[indexB]);
            return list;
        }
    }
}