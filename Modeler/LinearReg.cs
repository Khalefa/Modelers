using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace Modeler
{
    class LinearReg : Regression
    {

        static public double[] Regress(double[] Y, double[,] X, double[] W)
        {
            int M = Y.Length;             // M = Number of data points
            int N = X.Length / M;         // N = Number of linear terms
            int NDF = M - N;              // Degrees of freedom
            // If not enough data, don't attempt regression
            if (NDF < 1)
            {
                return null;
            }
            double[,] V = new double[N, N];
            double[] C = new double[N];

            double[] B = new double[N];   // Vector for LSQ

            // Clear the matrices to start out
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    V[i, j] = 0;

            // Form Least Squares Matrix
            if (W != null)
            {
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        V[i, j] = 0;
                        for (int k = 0; k < M; k++)
                            V[i, j] = V[i, j] + W[k] * X[i, k] * X[j, k];
                    }
                    B[i] = 0;
                    for (int k = 0; k < M; k++)
                        B[i] = B[i] + W[k] * X[i, k] * Y[k];
                }
            }
            else
            {
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        V[i, j] = 0;
                        for (int k = 0; k < M; k++)
                            V[i, j] = V[i, j] + X[i, k] * X[j, k];
                    }
                    B[i] = 0;
                    for (int k = 0; k < M; k++)
                        B[i] = B[i] + X[i, k] * Y[k];
                }
            }
            // V now contains the raw least squares matrix
            if (!SymmetricMatrixInvert(V))
            {
                return null;
            }
            // V now contains the inverted least square matrix
            // Matrix multpily to get coefficients C = VB
            for (int i = 0; i < N; i++)
            {
                C[i] = 0;
                for (int j = 0; j < N; j++)
                    C[i] = C[i] + V[i, j] * B[j];
            }

            return C;
        }
        //Y is actual

        static bool SymmetricMatrixInvert(double[,] V)
        {
            int N = (int)Math.Sqrt(V.Length);
            double[] t = new double[N];
            double[] Q = new double[N];
            double[] R = new double[N];
            double AB;
            int K, L, M;

            // Invert a symetric matrix in V
            for (M = 0; M < N; M++)
                R[M] = 1;
            K = 0;
            for (M = 0; M < N; M++)
            {
                double Big = 0;
                for (L = 0; L < N; L++)
                {
                    AB = Math.Abs(V[L, L]);
                    if ((AB > Big) && (R[L] != 0))
                    {
                        Big = AB;
                        K = L;
                    }
                }
                if (Big == 0)
                {
                    return false;
                }
                R[K] = 0;
                Q[K] = 1 / V[K, K];
                t[K] = 1;
                V[K, K] = 0;
                if (K != 0)
                {
                    for (L = 0; L < K; L++)
                    {
                        t[L] = V[L, K];
                        if (R[L] == 0)
                            Q[L] = V[L, K] * Q[K];
                        else
                            Q[L] = -V[L, K] * Q[K];
                        V[L, K] = 0;
                    }
                }
                if ((K + 1) < N)
                {
                    for (L = K + 1; L < N; L++)
                    {
                        if (R[L] != 0)
                            t[L] = V[K, L];
                        else
                            t[L] = -V[K, L];
                        Q[L] = -V[K, L] * Q[K];
                        V[K, L] = 0;
                    }
                }
                for (L = 0; L < N; L++)
                    for (K = L; K < N; K++)
                        V[L, K] = V[L, K] + t[L] * Q[K];
            }
            M = N;
            L = N - 1;
            for (K = 1; K < N; K++)
            {
                M = M - 1;
                L = L - 1;
                for (int J = 0; J <= L; J++)
                    V[M, J] = V[J, M];
            }
            return true;
        }
        static double mean(double n)
        {
            return (n + 1) / 2;
        }
        static double stddev(double n)
        {
            return Math.Sqrt((n + 1) * (n - 1) / 12);
        }

        static double mean(double[] values)
        {
            return values.Average();
        }
        static double stddev(IEnumerable<int> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }
        static double stddev(double[] values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }

        static double corellation(double[] x, double[] y)
        {
            double xysum = 0;
            double x2sum = 0;
            double y2sum = 0;

            for (int i = 0; i < x.Length; i++)
            {
                xysum += x[i] * y[i];
                x2sum += x[i] * x[i];
                y2sum += y[i] * y[i];
            }
            return xysum / Math.Sqrt(x2sum * y2sum);
        }
        static double corellation(int n, IEnumerable<int> y)
        {
            double xysum = 0;
            double x2sum = 0;
            double y2sum = 0;
            IEnumerator<int> l = y.GetEnumerator();
            l.MoveNext();
            for (int i = 0; i < n; i++)
            {
                double yy = l.Current;
                xysum += (i + 1) * yy;
                x2sum += (i + 1) * (i + 1);
                y2sum += yy * yy;
            }
            return xysum / Math.Sqrt(x2sum * y2sum);
        }
        static public double[] OptSolve(IEnumerable<int> y)
        {
            double[] values = new double[2];

            int x = y.Count();
            double r = corellation(x, y);
            double b = r * stddev(y) / stddev(x);
            double a = y.Average() - b * mean(x);
            values[1] = b;
            values[0] = a;
            return values;
        }
        /*        static public double[] CalcError(double[] Y)
                {
                    double[,] coff = Regression.getArr(Y.Length);
                    double[] p = Solve(coff, Y);
                    if (Math.Abs(p[1]) < 0.1) return null;
                    double[] E = Regression.CalcError(coff, p, Y);
                    double deltaY = Y.Max() - Y.Min();
                    double deltaE = E.Max() - E.Min();
                    if (Math.Abs(deltaE) > Math.Abs(deltaY)) return null;
                    return E;
                }*/
        static public IEnumerable<int> CalcError(IEnumerable<int> Y)
        {
            double[] p = OptSolve(Y);
            if (Math.Abs(p[1]) < 0.1) yield break;
            //double[] E = new double[Y.Count()];
            double maxE = double.MinValue;
            double minE = double.MaxValue;
            //for (int i = 0; i < E.Length; i++)
            //IEnumerator<int> Y_itr = Y.GetEnumerator();
            //Y_itr.MoveNext();
            int[] Y_arr = Y.ToArray();
            for (int i = 0; i < Y_arr.Count(); i++)
            {
                double e = p[0] + p[1] * (i + 1) - Y_arr[i];
                if (e > maxE) maxE = e;
                if (e < minE) minE = e;
                //Y_itr.MoveNext();
            }
            double deltaY = Y.Max() - Y.Min();
            double deltaE = maxE - minE;
            if (Math.Abs(deltaE) > Math.Abs(deltaY)) yield break;
            for (int i = 0; i < Y_arr.Count(); i++)
            {
                int e = (int)(p[0] + p[1] * (i + 1) - Y_arr[i]);
                yield return e;
            }

        }

        static public double[] Solve(double[,] X, double[] Y)
        {
            int d0 = X.GetLength(0);
            int d1 = X.GetLength(1);

            double[,] Xt = new double[X.GetLength(1), X.GetLength(0)];
            double[] W = new double[Y.Length];
            for (int i = 0; i < d0; i++)
                for (int j = 0; j < d1; j++)
                    Xt[j, i] = X[i, j];

            return Regress(Y, Xt, null);
        }
    }
}
