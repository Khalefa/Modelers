using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace histogram
{
    class Histogram
    {
       static public long estimate(int[] d, int i, int part=1)
        {
            part p = LimitParition;
            if (part == 2) p = EquiParition;
            return Partion(d, i, p);
        }

        static List<int>[] LimitParition(int[] d, int limit)
        {
            //init the array
            List<List<int>> parts = new List<List<int>>();

            int m = d.Min();
            int[] d_sorted = d.OrderBy(a => a).ToArray();
            HashSet<int> l = new HashSet<int>();
            List<int> ll = new List<int>();
            for (int i = 0; i < d_sorted.Length; i++)
            {
                if (l.Contains(d_sorted[i]))
                {
                    ll.Add(d_sorted[i]);
                }
                else
                {
                    l.Add(d_sorted[i]);
                    ll.Add(d_sorted[i]);
                    if (l.Count() > limit)
                    {
                        parts.Add(ll);
                        l = new HashSet<int>();
                        ll = new List<int>();
                    }
                }
            }
            //
            if (ll.Count > 0)
            {
                parts.Add(ll);
            }
            return parts.ToArray();
        }
       static  List<int>[] EquiParition(int[] d, int paritions)
        {
            //init the array
            List<int>[] parts = new List<int>[paritions + 1];
            for (int i = 0; i < parts.Length; i++)
                parts[i] = new List<int>((int)d.Count() / paritions);

            int range = (int)((d.Max() - d.Min()) / paritions);

            int m = d.Min();
            int[] d_sorted = d.OrderBy(a => a).ToArray();
            for (int i = 0; i < d_sorted.Length; i++)
            {
                int l = (d_sorted[i] - m) / range;
                if (l > paritions) l = paritions;
                parts[l].Add(d_sorted[i]);
            }
            return parts;
        }
        public delegate List<int>[] part(int[] data, int part_no);
       static public long Partion(int[] d, int i, part p)
        {
            var m = p(d, i);
            long s = 0;
            foreach (List<int> item in m)
            {
                s += estimate(item.ToArray());
            }
            s += (int)Math.Ceiling(Math.Log(i, 2)) * d.Count();
            return s;
        }
        /*  public  long s_part_limit(int[] d, int i)
          {
              var m = Part_limit(d, i);
              long s = 0;
              foreach (List<int> item in m)
              {
                  s += estimate(item.ToArray());
              }
              s += (int)Math.Ceiling(Math.Log(m.Length, 2)) * d.Count();
              return s;
          }*/

        //public  long s_part_limit_2(int[] d, int i)
        //{
        //    var m = Part_limit(d, i);
        //    long s = 0;
        //    foreach (List<int> item in m)
        //    {
        //        s += estimate_2(item.ToArray());
        //    }
        //    s += (int)Math.Ceiling(Math.Log(m.Length, 2)) * d.Count();
        //    return s;
        //}
        public static long estimate(int[] x)
        {
            if (x.Length == 0) return 0;
            if (x.Max() > x.Min())
            {
                var l = (int)Math.Ceiling(Math.Log(x.Max() - x.Min(), 2));
                return l * x.Count();
            }
            return 1 + (int)Math.Ceiling(Math.Log(x.Count(), 2));
        }
    }
}
