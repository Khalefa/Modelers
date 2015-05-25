using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compress;
using histogram;
namespace histogram
{
    class Program
    {
        static double[] readfile(string filename, int part)
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
        static long getStorage(IEnumerable<int> num, ArrayList choics, int level)
        {
            ArrayList l3 = new ArrayList();
            ArrayList l4 = new ArrayList();

            if (num.Count() == 1) return 1;
            long[] s = new long[] { long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue };

            //raw storage
            s[0] = (long)Math.Ceiling(Math.Log(num.Max() - num.Min(), 2)) * num.Count();

            //delta
            int[] d = num.diff();
            // int[] dd = diff_all(num).ToArray();

            s[1] = (long)Math.Ceiling(Math.Log(d.Max() - d.Min() + 1, 2)) * (num.Count() - 1);
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
            //s[5] = huffman(num);

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

            if (choics != null)
            {
                if (minpos == 3)
                    choics.AddRange(l3);
                if (minpos == 4)
                    choics.AddRange(l4);

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
            return getStorage(x, a, 1);
            // return estimate(x);
        }

        static void FindBestModel(int[] data)
        {
            Console.WriteLine("direct:\t" + estimate(data) / data.Count());
            Console.WriteLine("delta direct:\t" + estimate(data.delta()) / data.Count());
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


                //long ts_2 = s_part_limit_2(data, k);
                long ts_limit, ts_equ;
                ts_limit = Histogram.estimate(data, k);
                ts_equ = Histogram.estimate(data, k, 2);
                long ts_hufman = Huffman.huffman(data);
                float data_length = data.Length;
                long ts_hufman_delta = Huffman.huffman(data.delta());
                Console.WriteLine("ts \t" + ts_equ / data_length + "\n" +
                    "limit\t" + ts_limit/ data_length + "\n" +
                    "huffman\t" + ts_hufman / data_length + "\n" +
                    "hufman delta" + ts_hufman_delta / data_length);

                //test compress and decompress
                Huffman ht = new Huffman();
                Huffman h = new Huffman();
                BitArray bits = ht.compress(data.delta());
                int[] o = ht.uncompress(bits);
                bool match = true;
                int x = 0;
                int[] dd = data.delta();
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
            double[] data = readfile("c:/data/hannes/preds-hourly.csv", 9);
            //double[] data = readfile("c:/data/khalefa/test_huffman1.txt", 0);
            var d = Array.ConvertAll(data, x => (int)x);
            FindBestModel(d);
            return;

        }

    }
}
