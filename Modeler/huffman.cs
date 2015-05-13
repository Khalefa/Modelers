using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Priority_Queue;
namespace Modeler
{
    public class Node:PriorityQueueNode
    {
        public string Symbol { get; set; }
        public int Frequency { get; set; }
        public Node Right { get; set; }
        public Node Left { get; set; }

        public List<bool> Traverse(string symbol, List<bool> data)
        {
            // Leaf
            if (Right == null && Left == null)
            {
                if (symbol.Equals(this.Symbol))
                {
                    return data;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<bool> left = null;
                List<bool> right = null;

                if (Left != null)
                {
                    List<bool> leftPath = new List<bool>();
                    leftPath.AddRange(data);
                    leftPath.Add(false);

                    left = Left.Traverse(symbol, leftPath);
                }

                if (Right != null)
                {
                    List<bool> rightPath = new List<bool>();
                    rightPath.AddRange(data);
                    rightPath.Add(true);
                    right = Right.Traverse(symbol, rightPath);
                }

                if (left != null)
                {
                    return left;
                }
                else
                {
                    return right;
                }
            }
        }
        public long TotalFrequency()
        {
            if (Right == null) return Frequency;
            else return Frequency + Right.TotalFrequency() + Left.TotalFrequency();
                
        }
    }
    public class HuffmanTree
    {
        //private List<Node> nodes = new List<Node>();
        private HeapPriorityQueue<Node> nodes =null;
        public Node Root { get; set; }
        public Dictionary<string, int> Frequencies = new Dictionary<string, int>();

        public long Size()
        {
            int size = 0;
          //  List<Node> ls = new List<Node>();

            //foreach (string x in Frequencies.Keys)
            //{
            //    List<bool> l = Root.Traverse(x, new List<bool>());
            //    size += l.Count() * Frequencies[x];

            //}

            return Root.TotalFrequency();
        }
        public void Build()
        {
            nodes=new HeapPriorityQueue<Node>(Frequencies.Count);
            foreach (KeyValuePair<string, int> symbol in Frequencies)
            {
                nodes.Enqueue(new Node() { Symbol = symbol.Key, Frequency = symbol.Value },symbol.Value);                   
            }

            while (nodes.Count > 1)
            {
             

                if (nodes.Count >= 2)
                {
                    // Take first two items
                 //   List<Node> taken = orderedNodes.Take(2).ToList<Node>();
                    Node a = nodes.Dequeue();
                    Node  b= nodes.Dequeue();
                    // Create a parent node by combining the frequencies
                    Node parent = new Node()
                    {
                        Symbol = "*",
                        Frequency = a.Frequency + b.Frequency,
                        Left = a,
                        Right = b
                    };

                    nodes.Enqueue(parent,parent.Frequency);
                }

            }
            this.Root = nodes.Dequeue();

        }

        public BitArray Encode(string[] source)
        {
            List<bool> encodedSource = new List<bool>();

            for (int i = 0; i < source.Length; i++)
            {
                List<bool> encodedSymbol = this.Root.Traverse(source[i], new List<bool>());
                encodedSource.AddRange(encodedSymbol);
            }

            BitArray bits = new BitArray(encodedSource.ToArray());

            return bits;
        }

        public string Decode(BitArray bits)
        {
            Node current = this.Root;
            string decoded = "";

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    if (current.Left != null)
                    {
                        current = current.Left;
                    }
                }

                if (IsLeaf(current))
                {
                    decoded += current.Symbol;
                    current = this.Root;
                }
            }

            return decoded;
        }

        public bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }

    }
    
}
