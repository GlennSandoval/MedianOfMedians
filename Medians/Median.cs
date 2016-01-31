using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Medians
{
    public static class Median
    {

        public static T FindMedian<T>(IEnumerable<T> list, Comparison<T> compare)
        {

            if (list == null || list.Count() == 0)
            {
                // Really sholuld provide at least 3 items.
                throw new ArgumentException("Must provide at least 1 item");
            }

            int count = list.Count();

            var right = Select(list, count / 2, compare);

            if (list.Count() % 2 != 0)
            {
                return right;
            }
            else
            {
                var left = Select(list, (list.Count() / 2) + 1, compare);
                return left;
            }
        }

        static T Select<T>(IEnumerable<T> list, int position, Comparison<T> compare)
        {
            if (list.Count() < 10)
            {
                var l = new List<T>(list);
                l.Sort();
                return l[position - 1];
            }

            var s = new List<List<T>>();

            int partitions = list.Count() / 5;

            List<T> wrapper = new List<T>(list);

            for (int i = 0; i < partitions; i++)
            {
                s.Add(new List<T>(wrapper.GetRange(i * 5, 5)));
            }

            List<T> medians = new List<T>();

            foreach (var sl in s)
            {
                medians.Add(Select(sl, 3, compare));
            }

            var medianOfMedians = Select(medians, list.Count() / 10, compare);

            List<T> l1 = new List<T>();
            List<T> l3 = new List<T>();

            foreach (var d in list)
            {
                if (compare(d, medianOfMedians) < 0)
                {
                    l1.Add(d);
                }
                else
                {
                    l3.Add(d);
                }
            }

            if (position <= l1.Count)
            {
                return Select(l1, position, compare);
            }
            else if (position > l1.Count + l3.Count)
            {
                return Select(l3, position - l1.Count - l3.Count, compare);
            }
            else
            {
                return medianOfMedians;
            }

        }

        static int FastSelect<T>(T[] list, int lo, int hi, int k) where T: IComparable
        {
            int n = hi - lo;
            if (n < 2)
                return lo;

            T pivot = list[lo + (k * 7919) % n]; // Pick a random pivot

            // Triage list to [<pivot][=pivot][>pivot]
            int nLess = 0, nSame = 0, nMore = 0;
            int lo3 = lo;
            int hi3 = hi;
            while (lo3 < hi3)
            {
                T e = list[lo3];
                int cmp = e.CompareTo(pivot);
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
                else {
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
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }

    }

}

