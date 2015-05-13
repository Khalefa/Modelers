using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Compresser
{
    class Program
    {
        static double[] readfile(string filename,int part)
        {
            List<double> l = new List<double>();
            StreamReader sr = new StreamReader(filename);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                var parts = line.Split(',');
                l.Add(Double.Parse(parts[part]));
            }

            sr.Close();
            return l.ToArray();
        }

        static BitArray compress(int[] data)
        {
            ICompress c = new Huffman();
            BitArray bits=c.compress(data);
            return bits;
        }
        static int[] uncompress(BitArray bits)
        {
            ICompress c = new Huffman();
            return c.uncompress(bits);            
        }
        static void Main(string[] args)
        {
            var data = Array.ConvertAll(readfile(@"test.txt"), x => (int)x);
           BitArray ba= compress(data);
           Console.WriteLine(ba.Length);
           int[] b = uncompress(ba);
            
        }
    }
}
