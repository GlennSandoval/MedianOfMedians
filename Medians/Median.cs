using System;
using System.Collections.Generic;
using System.Linq;

namespace Medians
{
    public static class Median
    {
        public static T FindMedian<T>(IEnumerable<T> list, Comparison<T> compare)
        {
            var values = new List<T>(list);
            if (!values.Any())
                // Really should provide at least 3 items.
                throw new ArgumentException("Must provide at least 1 item");

            var count = values.Count();

            var right = Select(values, count / 2, compare);

            if (values.Count() % 2 != 0)
            {
                return right;
            }
            else
            {
                var left = Select(values, values.Count() / 2 + 1, compare);
                return left;
            }
        }

        private static T Select<T>(IEnumerable<T> list, int position, Comparison<T> compare)
        {
            var values = new List<T>(list);
            if (values.Count() < 10)
            {
                var l = new List<T>(values);
                l.Sort();
                return l[position - 1];
            }

            var s = new List<List<T>>();

            var partitions = values.Count() / 5;

            var wrapper = new List<T>(values);

            for (var i = 0; i < partitions; i++) s.Add(new List<T>(wrapper.GetRange(i * 5, 5)));

            var medians = s.Select(sl => Select(sl, 3, compare)).ToList();

            var medianOfMedians = Select(medians, values.Count() / 10, compare);

            var l1 = new List<T>();
            var l3 = new List<T>();

            foreach (var d in values)
                if (compare(d, medianOfMedians) < 0)
                    l1.Add(d);
                else
                    l3.Add(d);

            if (position <= l1.Count)
                return Select(l1, position, compare);
            else if (position > l1.Count + l3.Count)
                return Select(l3, position - l1.Count - l3.Count, compare);
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
            var tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }
    }
}