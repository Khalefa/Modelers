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
        static double[] readbfile(string filename, int part)
        {
            BinaryReader sb = new BinaryReader(File.Open(filename,FileMode.Open));
            List<Int32> l = new List<int>();
            while (sb.BaseStream.Position != sb.BaseStream.Length)
            {
                int x = sb.ReadInt32();
                l.Add(x);
            }

            sb.Close();
            return Array.ConvertAll(l.ToArray(), z => (double)z);
        }
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
                //if (x > 100) x = x - 360;
                x = x * 100 *1000;
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
            s[0] = (long)Math.Ceiling(Math.Log(num.range(), 2)) * num.Count();

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
                var l = (int)Math.Ceiling(Math.Log((long)x.Max() - (long)x.Min(), 2));
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
            Console.WriteLine("delta direct:\t" + estimate(data.delta().delta()) / data.Count());
         
            int k = 1024 * 4;
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

                BitArray bit_array = Huffman.huffmanb(data.delta());
                byte [] bytes = new byte[bit_array.Length / 8 + ( bit_array.Length % 8 == 0 ? 0 : 1 )];
                bit_array.CopyTo( bytes, 0 );
                File.WriteAllBytes(@"C:/data/hannes/"+col+"hfdelta.bin", bytes);

                byte[] result = new byte[data.Length * sizeof(int)];
                Buffer.BlockCopy(data, 0, result, 0, result.Length);
            
                File.WriteAllBytes(@"C:/data/hannes/"+col+"data.bin", result);
                var d = data.delta();
                result = new byte[d.Length * sizeof(int)];  
                Buffer.BlockCopy(d, 0, result, 0, result.Length);
                File.WriteAllBytes(@"C:/data/hannes/"+col+"delta.bin", result);
            /*
                ts_limit = Histogram.estimate(data.delta(), k);
                ts_equ = Histogram.estimate(data.delta(), k, 2);

                Console.WriteLine("ts delta" + ts_limit);
                Console.WriteLine("ts equ" + ts_equ);
             * 
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
               
             */
            k = k * 2;
            
        }

        static int col = 10-1;
        static void freq(int[] d)
        {
            Dictionary<List<int>, int> f = new Dictionary<List<int>, int>();
            foreach (int x in d)
            {
                List<int> h = new List<int>();
                h.Add(x);
                if (f.ContainsKey(h))
                    f[h]++;
                else f.Add(h, 1);

            }
          //  Tuple<int, int> t = new Tuple<string, int>("Hello", 4);

            for (int i = 0; i < d.Length - 1; i++)
            {
            //    Tuple<int, int> t = new Tuple<int, int>(d[i], d[i + 1]);
                List<int> h = new List<int>();
                h.Add(d[i]);
                h.Add(d[i+1]);
                if (f.ContainsKey(h))
                    f[h]++;
                else f.Add(h, 1);
            }

            List<KeyValuePair<List<int>, int>> myList = f.ToList();

            myList.Sort((firstPair, nextPair) =>
            {
                return nextPair.Value.CompareTo(firstPair.Value);
            }
            );
        }
        static void Main(string[] args)
        {
           double[] data = readfile("c:/data/hannes/preds-hourly.csv", col);

           var d = Array.ConvertAll(data, x => (int)x);
          // freq(d.delta());
           //return;

       //     double[] data = readbfile("c:/data/b.bat", 0);
            //double[] data = readfile("c:/data/khalefa/test_huffman1.txt", 0);
            
            
            FindBestModel(d);
            return;

        }

    }
}
