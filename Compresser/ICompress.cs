using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compresser
{
    interface ICompress
    {
          BitArray compress(int[] data);
         int[]  uncompress(BitArray bits);
    }
}
