using System;
using System.Linq;

public class LUDecomposition
{
    public readonly int[] Substitutions;
    public readonly double[,] LU;

    public LUDecomposition(double[,] mat, bool partialPivoting = false)
    {
        var n = mat.GetLength(0);
        if (n != mat.GetLength(1))
        {
            throw new ArgumentException("The matrix must be square.");
        }
        mat = mat.Clone() as double[,];

        var lu = new double[n, n];
        var subs = Enumerable.Range(0, n).ToArray();

        for (var i = 0; i < n; i++)
        {
            if (partialPivoting)
            {
                var maxRow = i;
                for (var j = i + 1; j < n; j++)
                {
                    if (Math.Abs(mat[subs[j], i]) > Math.Abs(mat[subs[maxRow], i]))
                    {
                        maxRow = j;
                    }
                }
                swap(ref subs[i], ref subs[maxRow]);
            }
            lu[i, i] = 1.0 / mat[subs[i], i];
            for (var j = i + 1; j < n; j++)
            {
                lu[i, j] = mat[subs[i], j];
                lu[j, i] = mat[subs[j], i] * lu[i, i];
            }
            for (var k = i + 1; k < n; k++)
            {
                for (var l = i + 1; l < n; l++)
                {
                    mat[subs[k], l] -= lu[k, i] * lu[i, l];
                }
            }
        }

        Substitutions = subs;
        LU = lu;
    }

    public double[] SolveLinearEquations(double[] vec)
    {
        if (vec.Length != Size)
        {
            throw new ArgumentException("Invalid size.");
        }
        int i, j;
        var y = new double[Size];
        var x = new double[Size];
        for (i = 0; i < Size; i++)
        {
            y[i] = vec[Substitutions[i]];
            for (j = 0; j < i; j++)
            {
                y[i] -= LU[i, j] * y[j];
            }
        }
        for (i = Size - 1; i >= 0; i--)
        {
            x[i] = y[i];
            for (j = Size - 1; j > i; j--)
            {
                x[i] -= LU[i, j] * x[j];
            }
            x[i] *= LU[i, i];
        }
        for (i = 0; i < Size; i++)
        {
            y[i] = x[Substitutions[i]];
        }
        return x;
    }

    static void swap<T>(ref T a, ref T b)
    {
        T temp;
        temp = a;
        a = b;
        b = temp;
    }

    public int Size
    {
        get
        {
            return Substitutions.Length;
        }
    }

    public double GetElementOfL(int i, int j)
    {
        if (i < 0 || j < 0 || i >= Size || j >= Size)
        {
            throw new ArgumentOutOfRangeException();
        }
        if (i == j)
            return 1.0;
        else if (i < j)
            return 0.0;
        else
            return LU[i, j];
    }

    public double GetElementOfU(int i, int j)
    {
        if (i < 0 || j < 0 || i >= Size || j >= Size)
        {
            throw new ArgumentOutOfRangeException();
        }
        if (i == j)
            return 1.0 / LU[i, j];
        else if (i > j)
            return 0.0;
        else
            return LU[i, j];
    }

    public double[,] Restore()
    {
        var mat = new double[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                mat[i, j] = 0.0;
                for (int k = 0; k < Size; k++)
                {
                    mat[i, j] += GetElementOfL(i, k) * GetElementOfU(k, j);
                }
            }
        }
        return mat;
    }
}
