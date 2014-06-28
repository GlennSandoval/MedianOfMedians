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

        public static double FindMedian(IEnumerable<double> list)
        {

            if (list == null || list.Count() == 0)
            {
                throw new ArgumentException();
            }

            int count = list.Count();

            var right = Select(list, count / 2);

            if (list.Count() % 2 != 0)
            {
                return right;
            }
            else
            {
                var left = Select(list, (list.Count() / 2) + 1);
                return (right + left) / 2;
            }
        }

        static double Select(IEnumerable<double> list, int position)
        {
            if (list.Count() < 10)
            {
                List<double> l = new List<double>(list);
                l.Sort();
                return l[position-1];
            }

            var s = new List<List<double>>();

            int partitions = list.Count() / 5;

            List<double> wrapper = new List<double>(list);

            for (int i = 0; i < partitions; i++)
            {
                s.Add(new List<double>(wrapper.GetRange(i * 5, 5)));
            }

            List<double> medians = new List<double>();

            foreach (var sl in s)
            {
                medians.Add(Select(sl, 3));
            }

            double medianOfMedians = Select(medians, list.Count() / 10);

            List<double> l1 = new List<double>();
            List<double> l3 = new List<double>();

            foreach (var d in list)
            {
                if (d < medianOfMedians)
                {
                    l1.Add(d);
                }
                else if (d > medianOfMedians)
                {
                    l3.Add(d);
                }
            }

            if (position <= l1.Count)
            {
                return Select(l1, position);
            }
            else if (position > l1.Count + l3.Count)
            {
                return Select(l3, position - l1.Count - l3.Count);
            }
            else
            {
                return medianOfMedians;
            }

        }

    }

}

