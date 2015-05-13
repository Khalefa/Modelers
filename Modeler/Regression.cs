using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeler
{
    class Regression
    {
        public static double[,] getArr(int l)
        {
            double [,] m=new double[l,2];
            for (int i = 0; i < l; i++)
            {
                m[ i,0] = 1;
                m[ i,1] = i;
            }
            return m;
        }
        static public double[] CalcError(double[,] X, double[] C, double[] Y)
        {
            int M = Y.Length;             // M = Number of data points
            int N = X.Length / M;         // N = Number of linear terms

            double[] Ycalc = new double[M];
            double[] E = new double[M];
            for (int k = 0; k < M; k++)
            {
                Ycalc[k] = 0;
                for (int i = 0; i < N; i++)
                    Ycalc[k] = Ycalc[k] + C[i] * X[ k,i];
                E[k] = Ycalc[k] - Y[k];

            }
            return E;
        }
    }
}
