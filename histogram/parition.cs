using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace histogram
{
    class median
    {
        //partition the array into 
        public static Tuple<List<int>, List<int>> p(int[] d, int med)
        {
            List<int> l = new List<int>();
            List<int> r = new List<int>();
            foreach (int x in d)
            {
                if (x > med) l.Add(x);
                else r.Add(x);
            }
            return Tuple.Create(l, r);
        }
    }
}
