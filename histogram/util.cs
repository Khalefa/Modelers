using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace histogram
{
    public static class util
    {
        public static double Median(this int[] list)
        {
            List<int> orderedList = list
                .OrderBy(numbers => numbers)
                .ToList();

            int listSize = orderedList.Count;
            double result;

            if (listSize % 2 == 0) // even
            {
                int midIndex = listSize / 2;
                result = ((orderedList.ElementAt(midIndex - 1) +
                           orderedList.ElementAt(midIndex)) / 2);
            }
            else // odd
            {
                double element = (double)listSize / 2;
                element = Math.Round(element, MidpointRounding.AwayFromZero);

                result = orderedList.ElementAt((int)(element - 1));
            }

            return result;
        }
        static public int[] diff(this IEnumerable<int> data)
        {
            int[] a = new int[2];//data.Length - 1];
            IEnumerator<int> l = data.GetEnumerator();
            //l.Reset();
            l.MoveNext();
            int x = l.Current;
            int y;
            int max = int.MinValue;
            int min = int.MaxValue;

            while (l.MoveNext())
            {
                y = l.Current - x;
                if (y > max) max = y;
                if (y < min) min = y;
                x = l.Current;
            }
            a[0] = max;
            a[1] = min;
            return a;
        }
        static public int[] delta(this int[] x)
        {
            int[] y = new int[x.Length - 1];
            for (int i = 1; i < x.Length; i++)
            {
                y[i - 1] = x[i] - x[i - 1];
            }
            return y;
        }
                
    }
}
