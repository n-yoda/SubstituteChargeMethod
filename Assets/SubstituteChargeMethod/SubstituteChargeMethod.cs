using System;

public class SubstituteChargeMethod
{
    public delegate Vector2D ParametricBound(double t);

    public delegate double ParametricDirichlet(double t);

    public const double MinDistance = 1.00001;
    public readonly double[] Qs;
    public readonly Vector2D[] Charges;
    public readonly Vector2D[] Samples;
    private ParametricBound bound;
    private ParametricDirichlet dirichlet;

    public SubstituteChargeMethod(
        ParametricBound counterclock, ParametricDirichlet dirichlet,
        int chargeCount, double distance)
    {
        // determin charge and sample positions
        Charges = new Vector2D[chargeCount];
        Samples = new Vector2D[chargeCount];
        var sampleValues = new double[chargeCount];
        var unit = 1.0 / chargeCount;
        var dt = unit * 0.5;
        for (var i = 0; i < chargeCount; i++)
        {
            Samples[i] = counterclock(unit * i);
            sampleValues[i] = dirichlet(unit * i);
            var diff = counterclock(unit * i + dt)
                       - counterclock(unit * i - dt);
            var dir = new Vector2D(diff.Y, -diff.X).Normalized;
            Charges[i] = Samples[i] + dir * distance;
        }

        // calculate Q values
        var mat = new double[chargeCount, chargeCount];
        var p2m = -1.0 / (Math.PI * 2.0);
        for (var i = 0; i < chargeCount; i++)
        {
            for (var j = 0; j < chargeCount; j++)
            {
                mat[i, j] = Math.Log((Samples[i] - Charges[j]).Length) * p2m;
            }
        }
        var lu = new LUDecomposition(mat);
        Qs = lu.SolveLinearEquations(sampleValues);

        this.bound = counterclock;
        this.dirichlet = dirichlet;
    }

    public double CalcValueAt(Vector2D pos)
    {
        var ans = 0.0;
        var p2m = -1.0 / (Math.PI * 2.0);
        for (var i = 0; i < Qs.Length; i++)
        {
            ans += Math.Log((pos - Charges[i]).Length) * p2m * Qs[i];
        }
        return ans;
    }

    public double CalcMaxError(double delta)
    {
        var max = 0.0;
        var t = 0.0;
        while (t < 1.0)
        {
            max = Math.Max(Math.Abs(CalcValueAt(bound(t)) - dirichlet(t)), max);
            t += delta;
        }
        return max;
    }
}
