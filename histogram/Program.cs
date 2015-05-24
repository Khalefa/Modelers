using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compresser;
namespace histogram
{
    class Program
    {
        static double[] readfile(string filename,int part)
        {
            List<double> l = new List<double>();
            
            StreamReader sr = new StreamReader(filename);
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                var parts = line.Split(',');
                var x = Double.Parse(parts[part]);
                if (x > 100) x = x - 360;
                x = x * 100000;
                l.Add(x);
            }

            sr.Close();
            return l.ToArray();
        }
        public static double Median(int[] list)
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
        static int[] diff(IEnumerable<int> data)
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
        static long getStorage(IEnumerable<int> num, ArrayList choics, int level)
        {
            ArrayList l3 = new ArrayList();
            ArrayList l4 = new ArrayList();

            if (num.Count() == 1) return 1;
            long[] s = new long[] { long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue };

            //raw storage
            s[0] = (long)Math.Ceiling(Math.Log(num.Max() - num.Min(), 2)) * num.Count();

            //delta
            int[] d = diff(num);
            // int[] dd = diff_all(num).ToArray();
            
            s[1] = (long)Math.Ceiling(Math.Log(d.Max() - d.Min()+1, 2)) * (num.Count() - 1) ;
            //create regression model
            //dictionary
            IEnumerable<int> data = num.Distinct();
            long t = num.Count() * (long)Math.Ceiling(Math.Log(data.Count(), 2));
            if (level >= 1)
            {
                s[3] = getStorage(data, l3, level - 1) + t;
                data = data.OrderBy(a => a);
                s[4] = getStorage(data, l4, level - 1) + t;
            }
            //if (choics != null)
            //    s[5] = huffman(num);

            int minpos = 0;
            long min = s[minpos];
            for (int i = 0; i < s.Length; i++)
            {
                if (min > s[i])
                {
                    min = s[i];
                    minpos = i;
                }
            }
            if (minpos == 3 || minpos == 4)
            {
               // Console.WriteLine("getstorage\t" + "level\t" + level + "\t#" + num.Count() + "\tdir" + (min - t) + "\t" + "size" + t);
            }
            else
            {
               // Console.WriteLine("getstorage\t" + "level\t" + level + "\t#" + num.Count() + "\tsize" + (min));

            }
            if (choics != null)
            {
                if (minpos == 3)
                    choics.AddRange(l3);
                if (minpos == 4) choics.AddRange(l4);

                choics.Add(minpos);
            }
            return min;
        }
       
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
        public static long estimate_2(int[] x)
        {
            ArrayList a = new ArrayList();
            return getStorage(x,a,1);
           // return estimate(x);
        }
        static public List<int>[] Part_limit(int[] d, int limit)
        {
            //init the array
            List<List<int>> parts = new List<List<int>>();

            int m = d.Min();
            int[] d_sorted = d.OrderBy(a => a).ToArray();
            HashSet<int> l = new HashSet<int>();
            List<int> ll=new List<int>();
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

        //partition the array into 
        static public List<int>[] Part(int[] d, int paritions)
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
        public static Tuple<List<int>, List<int>> parition(int[] d, int med)
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
        public static long s_part(int[] d, int i)
        {
            var m = Part(d, i);
            long s = 0;
            foreach (List<int> item in m)
            {
                s += estimate(item.ToArray());
            }
            s += (int)Math.Ceiling(Math.Log(i, 2)) * d.Count();                    
            return s;
        }
        public static long s_part_limit(int[] d, int i)
        {
            var m = Part_limit(d, i);
            long s = 0;
            foreach (List<int> item in m)
            {
                s += estimate(item.ToArray());
            }
            s+=(int)Math.Ceiling(Math.Log(m.Length, 2)) * d.Count();                    
            return s;
        }

        public static long s_part_limit_2(int[] d, int i)
        {
            var m = Part_limit(d, i);
            long s = 0;
            foreach (List<int> item in m)
            {
                s += estimate_2(item.ToArray());
            }
            s += (int)Math.Ceiling(Math.Log(m.Length, 2)) * d.Count();
            return s;
        }
        static long huffman(int[]data)
        {
            Huffman c = new Huffman();
            BitArray bits = c.compress(data);
            return bits.Count;
        }
        static public int[] delta( int[] x)
        {
            int[] y = new int[x.Length - 1];
            for (int i = 1; i < x.Length; i++)
            {
                y[i - 1] = x[i] - x[i - 1];
            }
            return y;
        }
       static  void FindBestModel(int[] data){
            Console.WriteLine("direct:\t" + estimate(data) / data.Count());
            Console.WriteLine("delta direct:\t" + estimate(delta(data)) / data.Count());
            //aas = aas / 8;
            //Console.WriteLine(aas );
            /*var data_c = d. Distinct().Count();
            var t = parition(d, (int)Median(d));
            Console.WriteLine((estimate(t.Item1.ToArray()) + estimate(t.Item2.ToArray())) / data.Count());*/
            /*for (int i = 1; i <= 300; i *= 2)
            {
                long s = s_part(d, i);
                Console.WriteLine(i + "\t" + s + "\t" + s / data.Length);
            }*/
            int k = 1024 * 4;
            //Console.WriteLine();
            //for (int j = 1; j<30; j++)
            {

                long ts = s_part_limit(data, k);
                long ts_2 = s_part_limit_2(data, k);
                long ts_hufman = huffman(data);
                float data_length = data.Length;
                long ts_hufman_delta = huffman(delta(data));
                Console.WriteLine("ts \t" + ts  / data_length + "\n" +
                    "limit\t" + ts_2 / data_length + "\n" +
                    "huffman\t" + ts_hufman / data_length + "\n" +
                    "hufman delta" + ts_hufman_delta / data_length);

                //test compress and decompress
                Huffman ht = new Huffman();
                Huffman h = new Huffman();
                BitArray bits = ht.compress(delta(data));
                int[] o = ht.uncompress(bits);
                bool match = true;
                int x = 0;
                int[] dd = delta(data);
                for (int i = 0; i < o.Length; i++)
                {
                    if (dd[i] != o[i])
                    {
                        match = false; x++;
                    }

                }
                if (match)
                    Console.Write("Match is sucsseful");
                else
                    Console.WriteLine("Error in huffman encoding");
                k = k * 2;
            }
        }

        static void Main(string[] args)
        {
            double[] data = readfile("c:/data/hannes/preds-hourly.csv",8);
            var d = Array.ConvertAll(data, x => (int)x);
            FindBestModel(d);
            return;
           //double[] data = readfile("c:/data/khalefa/test_huffman1.txt", 0);
            
            d = delta(d);
            Huffman h = new Huffman();
            Huffman hh = new Huffman();
            var bits = h.compress(d);
            var xbits = hh.uncompress(bits,null);
            bool match = true;
            int xx = 0;
            for (int i = 0; i < d.Length; i++)
            {
                if (xbits[i] != d[i])
                {
                    match = false; xx++;
                }

            }
            if (match)
                Console.Write("Match is sucsseful");
            else
                Console.WriteLine("Error in huffman encoding");
             
        }

    }
}
