using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
namespace Modeler
{
    public class Program
    {
        static double[] readfile(string filename)
        {
            List<double> l = new List<double>();
            StreamReader sr = new StreamReader(filename);
            while (!sr.EndOfStream)
            {
                l.Add(Double.Parse(sr.ReadLine()));
            }

            sr.Close();
            return l.ToArray();
        }
        public static double Median(IEnumerable<double> list)
        {
            List<double> orderedList = list
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
        // start with mod
        // L[s,s]=1

        /*  static void writefile(double[] d, string filename)
          {
                 StreamWriter sw = new StreamWriter(filename);
              for(int i=0;i<d.Length;i++)
              {
                  sw.WriteLine(d[i]);
              }
              sw.Close();
            
          }*/
        static void writefile(dynamic o, string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            foreach (var c in o)
            {
                sw.WriteLine(c);
            }
            sw.Close();
        }
        //we may try huffman enconding 
        // 0 


        static void analysis(double[] d)
        {
            //unique values
            int ucount = d.Select(a => Math.Abs(a)).Distinct().Count();
            var x = from i in d
                    group i by (int)i into gr
                    orderby gr.Count() descending
                    select (new { Value = gr.Key, Count = gr.Count() })
                     ;

            int count = d.Count();
            Console.WriteLine(ucount + "  " + count + " " + ucount * 100.0 / count);

            foreach (var xx in x.Take(10))
            {
                Console.WriteLine(xx.Value + " " + xx.Count);
            }

            var abs_d = d.Select(a => Math.Abs(a));
            Console.WriteLine("base data " + (d.Max() - d.Min()) + " " + Math.Log(d.Max() - d.Min(), 2));
            /*Console.WriteLine("abs data " + (abs_d.Max() - abs_d.Min()) + " " + Math.Log(abs_d.Max() - abs_d.Min()));
            var diff_d = diff(d);
             abs_d = diff_d.Select(a => Math.Abs(a));
            Console.WriteLine("diff data " + (diff_d.Max() - diff_d.Min()) + " " + Math.Log(diff_d.Max() - diff_d.Min()));
            Console.WriteLine("diff abs data " + (abs_d.Max() - abs_d.Min()) + " " + Math.Log(abs_d.Max() - abs_d.Min()));
            */
            //  Console.WriteLine(changeSign(d));
            // Console.WriteLine(changeSignNotinSequence(d));


        }
        static IEnumerable<int> diff_all(IEnumerable<int> data)
        {
            int x = data.First();
            foreach (int y in data)
            {
                yield return y - x;
                x = y;
            }

        }
        //get the max and min
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
        static int[] sign(double[] data)
        {
            var r = from d in data
                    select Math.Sign(d);
            return r.ToArray();
        }
        static int changesign(double x, double y)
        {
            if (x == 0 || y == 0) return 0;
            if (x > 0 && y > 0) return 0;
            if (x < 0 && y < 0) return 0;
            return 1;
        }
        static int changeSign(double[] d)
        {
            int c = 0;
            for (int i = 1; i < d.Length; i++)
                c += changesign(d[i - 1], d[i]);
            return c;
        }
        static int changeSignNotinSequence(double[] d)
        {
            int c = 0;
            for (int i = 1; i < d.Length; i++)
                c += changesign(d[i - 1], -d[i]);
            return c;
        }
        static long huffman(IEnumerable<int> num)
        {
            HuffmanTree h = new HuffmanTree();

            h.Frequencies = new Dictionary<string, int>();
            foreach (int x in num)
            {
                if (h.Frequencies.ContainsKey(x.ToString()))
                    h.Frequencies[x.ToString()]++;
                else
                    h.Frequencies.Add(x.ToString(), 1);
            }
            h.Build();
            long s = h.Size() - h.Root.Frequency;
            //estimate the direcoty size

            long dir = getStorage(num.Distinct(), null
                , 0);

            //(h.Frequencies.Count()*(int)Math.Ceiling(Math.Log(num.Max()-num.Min(),2)));
            Console.WriteLine("huffman"+num.Count()+"\tdir\t" + dir + "\tsize" + s);
            return s + dir;

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
            s[1] = (long)Math.Ceiling(Math.Log(d.Max() - d.Min(), 2)) * (num.Count() - 1) + (long)Math.Ceiling(Math.Log(Math.Abs(d.Min()), 2));
            //create regression model
            IEnumerable<int> CL = LinearReg.CalcError(num);

            if (CL.Count() != 0)
                s[2] = sizeof(double) * 2 * 8 + getStorage(CL, choics, level - 1);

            //dictionary
            IEnumerable<int> data = num.Distinct();
            long t = num.Count() * (long)Math.Ceiling(Math.Log(data.Count(), 2));
            if (level >= 1)
            {
                s[3] = getStorage(data, l3, level - 1) + t;
                data = data.OrderBy(a => a);
                s[4] = getStorage(data, l4, level - 1) + t;
            }
            if (choics != null)
                s[5] = huffman(num);

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
                Console.WriteLine("getstorage\t"+"level\t"+level+"\t#"+num.Count() + "\tdir" + (min - t) + "\t" + "size" + t);
            }
            else
            {
                Console.WriteLine("getstorage\t" + "level\t" + level + "\t#" + num.Count() + "\tsize" + (min) );

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
        static void Test()
        {
            double[] d = readfile("data.txt");

            for (int i = 1; i < 5; i++)
            {
                ArrayList choics = new ArrayList();

                long size = getStorage(Array.ConvertAll(d, x => (int)x /64), choics, i);
                Console.WriteLine(i + " " + size + " " + size * 1.0 / d.Count());
                foreach (int x in choics) { Console.Write(x + ":"); }
                Console.WriteLine();
            }

        }

        static void TestHuffman()
        {
            long t = huffman(new int[] { 1, 1, 1, 1,1,1,1,
                                2,2,2,2,            3 ,3,3,3,
            4,4,4,            5,5,
            6,6,
            7,7,
            8,8,
            9,9,
            10,10,
            11,
            12,
            13,
            14,15,16}


        );


        }
        /*static void olff()
        {
            double[] d = new double[2];
            var x = d.Distinct().OrderBy(a => a);
            int bits = (int)Math.Ceiling(Math.Log(x.Max() - x.Min(), 2));
            //Console.WriteLine( bits);
            //Console.WriteLine(x.Count());
            //we can either use
            //1-dictiory
            //2-delta

            int bits_s = x.Count() * bits + (int)Math.Ceiling(Math.Log(x.Count(), 2)) * d.Length;
            if (bits_s < bits * d.Length)
            {
                Console.Write("Use dictionary");
            }
            bits_s = Math.Min(bits_s, bits * d.Length);
            Console.WriteLine(bits_s / 8.0 / 1024 / 1024);

            var s = diff(x.ToArray());
            var xx = from i in s
                     group i by (int)i into gr
                     orderby gr.Count() descending
                     select (new { Value = gr.Key, Count = gr.Count() });

            writefile(xx, "diffs.txt");


        }*/
        static void Main(string[] args)
        {
            //t();
            Test();
            //TestHuffman();
        }
        static void t()
        {
            int[] r = new int[] { 3, 5, 7, 9, 11, 13 };
            var E = LinearReg.CalcError(r);
        }
        static void old(string[] args)
        {
            double[] d = readfile("data.txt");

            var t = from i in d
                    group i by (int)i into gr
                    orderby gr.Key descending
                    select (new { Value = gr.Key, Count = gr.Count() });

            analysis(d);
            // writefile(t,"freq.txt");
            var s = d.OrderByDescending(a => a).Distinct().ToArray();
            double[,] C = Regression.getArr(s.Length);
            //   d = d.Select(a => Math.Floor(Math.Abs(a))).ToArray();
            double[] CC = ChebReg.Solve(C, s);
            double[] CL = LinearReg.Solve(C, s);
            double[] E1 = Regression.CalcError(C, CC, s);
            double[] E2 = Regression.CalcError(C, CL, s);

            //E2 = E2.Select(a => Math.Floor(Math.Abs(a))).ToArray();
            var de1 = E1.Max() - E1.Min();
            var de2 = E2.Max() - E2.Min();
            Console.WriteLine(de1 + " " + Math.Log(de1, 2));
            Console.WriteLine(de2 + " " + Math.Log(de2, 2));
            var x = from e in E2
                    orderby e descending
                    select e;
            var x1 = x.Take(10);
            x = from e in E2
                orderby e ascending
                select e;
            var x2 = x.Take(10);

            //  writefile(diff(d), "diff.txt");

            //   writefile(sign(d), "sign.txt");

        }
    }
}
