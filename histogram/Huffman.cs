using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Compresser
{
    class Huffman 
    {
        public HuffmanTree ht = new HuffmanTree();
        public BitArray compress(int[] data)
        {
            ht.Frequencies = new Dictionary<int, int>();
            var sorted_data = data.OrderBy(x => x);
            foreach (int x in sorted_data)
            {
                if (ht.Frequencies.ContainsKey(x))
                    ht.Frequencies[x]++;
                else
                    ht.Frequencies.Add(x, 1);
            }

            ht.Build();
            //encode
            BitArray bits = ht.Encode(data);
            return bits;
        }
        int[] Decode(BitArray bits, Dictionary<int, int> freqd)
        {
            int l = bits.subset(0, 32).ToBinary();
            int code_bit = bits.subset(32, 32).ToBinary();
            int data_bit = bits.subset(32 + 32, 32).ToBinary();
            int min = bits.subset(32 + 32 + 32, 32).ToBinary();
            //read dictonary
            Dictionary< string,int> code_num = new Dictionary<string,int>();
            int indx = 32 * 4;
           //  HuffmanTree ht = new HuffmanTree();
             ht.Frequencies = new Dictionary<int, int>();
            for (int i = 0; i < l; i++)
            {
                int number = bits.subset(indx, data_bit).ToBinary() + min;
                indx += data_bit;
                int  freq = bits.subset(indx, code_bit).ToBinary();
                indx += code_bit;
                ht.Frequencies.Add(number, freq);
                //code_num.Add(code, number);
                //ht.AddCode(code,number);
            }
            if(freqd!=null)
            ht.Frequencies = freqd;
            ht.Build();
            List<Node> ns = ht.GetLeaves();
            foreach (Node n in ns)
            {
                code_num.Add(n.getCode().String(), n.Symbol);
            }
            List<int> data=new List<int>();
            List<bool> code = new List<bool>();
            for (;indx<bits.Count ; indx++)
            {
                code.Add(bits[indx]);
                if (code_num.ContainsKey(code.String()))
                {
                    data.Add(code_num[code.String()]);
                    code.Clear();
                }
            }
            
            return data.ToArray();
        }

        public int[] uncompress(System.Collections.BitArray bits, Dictionary<int, int> freq=null)
        {
            return Decode(bits,freq );
        }
    }
    class Node : PriorityQueueNode
    {
        public int Symbol { get; set; }
        public int frequency { get; set; }
        public List<bool> Code { get; set; }
        public Node Right { get; set; }
        public Node Left { get; set; }
        public Node Parent { get; set; }
        public bool IsLeaf()
        {
            return (Left == null && Right == null);
        }
        public List<bool> getCode()
        {
            if (this.Code != null) return this.Code;
            List<bool> code = new List<bool>();
            Node x = this;

            while (x.Parent != null)
            {
                if (x.Parent.Left == x)
                    code.Add(false);
                else
                    if (x.Parent.Right == x)
                        code.Add(true);

                x = x.Parent;
            }
            code.Reverse();
            this.Code = code;
            return code;
        }
        public List<bool> Traverse(int symbol, List<bool> data)
        {
            // Leaf
            if (Right == null && Left == null)
            {
                if (symbol == Symbol)
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
        public long Frequency()
        {
            if (Right == null) return frequency;
            else return frequency + Right.Frequency() + Left.Frequency();
        }
    }
    class HuffmanTree
    {
        private HeapPriorityQueue<Node> nodes = null;
        public Node Root { get; set; }
        public Dictionary<int, int> Frequencies = new Dictionary<Int32, int>();
        public Dictionary<int, List<bool>> codes = new Dictionary<int, List<bool>>();
        public List<Node> GetLeaves()
        {
            List<Node> leaves = new List<Node>();
            Queue<Node> nodes = new Queue<Node>();
            nodes.Enqueue(Root);
            while (nodes.Count > 0)
            {
                Node n = nodes.Dequeue();
                if (n.IsLeaf())
                    leaves.Add(n);
                else
                {
                    if (n.Right != null) nodes.Enqueue(n.Right);
                    if (n.Left != null) nodes.Enqueue(n.Left);
                }
            }
            return leaves;
        }
        class NodePair { 
            public Node N{get; set;}
            public int level { get; set; }            
            }
        public void Compress()
        {
            Stack<NodePair> nodes = new Stack<NodePair>();
            nodes.Push(new NodePair { N = Root, level = 0 });
            while (nodes.Count > 0)
            {
                NodePair np = nodes.Pop();
                Node n = np.N;
                int lvl = np.level;
                int l = 0;
                if (n.Right != null) l++;
                if (n.Left != null) l++;
                if (l == 1)
                {
                    if (n.Left == null)
                    {
                        n.Code = n.Right.Code.Take(lvl).ToList();
                        n.Right = null;
                    } else
                    if (n.Right == null)
                    {
                        n.Code = n.Left.Code.Take(lvl).ToList();
                        n.Left = null;
                    }
                }
                else
                {
                    if (n.Right != null) nodes.Push(new NodePair { N = n.Right, level = lvl + 1 });
                    if (n.Left != null) nodes.Push(new NodePair { N = n.Left, level = lvl + 1 });
                }
            }

        }
        public void AddCode(List<bool> code, int symbol)
        {
            if (Root == null)
                Root = new Node();
            Node p = Root;
            for (int i = 0; i < code.Count; i++)
            {
                Node x;
                if (code[i] == true)
                    x = p.Left;
                else x = p.Right;
                if (x == null)
                {
                    x = new Node();
                    if (code[i] == true)
                        p.Left = x;
                    else p.Right = x;
                }
                x.Symbol = symbol;
                x.Code = code;
                p = x;
            }
            
        }
        public long Size()
        {
            return Root.Frequency();
        }

        public void Build()
        {
            nodes = new HeapPriorityQueue<Node>(Frequencies.Count);
            foreach (KeyValuePair<Int32, int> symbol in Frequencies)
            {
                nodes.Enqueue(new Node() { Symbol = symbol.Key, frequency = symbol.Value }, symbol.Value);
            }

            while (nodes.Count >= 2)
            {
                //get the lowest two frequencies
                Node a = nodes.Dequeue();
                Node b = nodes.Dequeue();
                // Create a parent node by summing the frequencies
                Node parent = new Node()
                {
                    frequency = a.frequency + b.frequency,
                    Left = a,
                    Right = b
                };
                //adjust parent
                a.Parent = parent;
                b.Parent = parent;
                nodes.Enqueue(parent, parent.frequency);
            }
            this.Root = nodes.Dequeue();
        }
        List<bool> EncodeDict(int symbol_bits, int code_bit)
        {
            List<bool> dict = new List<bool>();
            int min = codes.Keys.Min();
            foreach (int x in codes.Keys.OrderBy(x=>x))
            {
                dict.AddRange((x - min).tobinary(symbol_bits));
                dict.AddRange(Frequencies[x].tobinary(code_bit));
            }
            return dict;
        }

        public BitArray Encode(int[] data)
        {
            List<bool> encodedData = new List<bool>();
            List<bool> encodedSource = new List<bool>();
            //encode sybmols
            int len = 0;
            List<Node> leaves = GetLeaves();
            foreach (Node x in leaves)
            {
                var t = x.getCode();
                if (t.Count() > len) len = t.Count;
                codes.Add(x.Symbol, x.getCode());
            }
            for (int i = 0; i < data.Length; i++)
            {
                encodedData.AddRange(codes[data[i]]);
            }
            int data_bits = (int)Math.Ceiling(Math.Log(data.Max() - data.Min(), 2));            
            int code_bits= (int)Math.Ceiling(Math.Log(Frequencies.Values.Max(),2));

            encodedSource.AddRange(codes.Count.tobinary(32)); //add the number of item
            encodedSource.AddRange(code_bits.tobinary(32)); //add the number of item
            encodedSource.AddRange(data_bits.tobinary(32)); //add the number of item
            encodedSource.AddRange(data.Min().tobinary(32)); //add minmum
            var dict = EncodeDict(data_bits, code_bits);
            encodedSource.AddRange(dict);
            encodedSource.AddRange(encodedData);

            BitArray bits = new BitArray(encodedSource.ToArray());
            return bits;
        }
        
        public bool IsLeaf(Node node)
        {
            return node.IsLeaf();
        }



    }

}
