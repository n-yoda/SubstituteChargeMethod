using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public static class Test
{
    static public double Radius = 10;
    static public double Height = 1;
    public const int Lattice = 128;

    public static Vector2D SquareBound(double t)
    {
        if (t <= 0.25)
        {
            return new Vector2D(Radius, t * 8 * Radius - Radius);
        }
        else if (t <= 0.5)
        {
            return new Vector2D(Radius * 3 - t * 8 * Radius, Radius);
        }
        else if (t <= 0.75)
        {
            return new Vector2D(-Radius, Radius * 5 - t * 8 * Radius);
        }
        else
        {
            return new Vector2D(t * 8 * Radius - Radius * 7, -Radius);
        }
    }

    public static double SquareDirichlet(double t)
    {
        var result = 0.0;
        if (t <= 0.25)
        {
            result = Math.Sin(t * 8 * Math.PI);
        }
        else if (t <= 0.5)
        {
            result = Math.Sin(t * 4 * Math.PI);
        }
        else if (t <= 0.75)
        {
            result = Math.Sin(t * 8 * Math.PI);
        }
        else
        {
            result = Math.Sin(t * 4 * Math.PI);
        }
        return result * Height;
    }

    public static bool IsInsideSquareBound(Vector2D x)
    {
        return Math.Abs(x.X) <= Radius && Math.Abs(x.Y) <= Radius;
    }

    public static Vector2D CircleBound(double t)
    {
        var theta = t * Math.PI * 2.0;
        return new Vector2D(Math.Cos(theta), Math.Sin(theta)) * Radius;
    }

    public static double CircleDirichlet(double t)
    {
        var theta = t * Math.PI * 2.0;
        return Math.Sin(theta * 4) * Height;
    }

    public static bool IsInsideCircleBound(Vector2D x)
    {
        return Vector2D.Dot(x, x) <= Radius * Radius;
    }

    [MenuItem("Test/Square %#d")]
    public static void TestSquare()
    {
        TestHoge(SquareBound, SquareDirichlet, IsInsideSquareBound);
    }

    [MenuItem("Test/Circle %#f")]
    public static void TestCircle()
    {
        TestHoge(CircleBound, CircleDirichlet, IsInsideCircleBound);
    }

    [MenuItem("Test/Square Errors by Charge Count")]
    public static void TestSquareErrors()
    {
        CalcCountErrors(SquareBound, SquareDirichlet, "Assets/Results/SquareErrors.txt");
    }

    [MenuItem("Test/Circle Errors by Charge Count")]
    public static void TestCircleErrors()
    {
        CalcCountErrors(CircleBound, CircleDirichlet, "Assets/Results/CircleErrors.txt");
    }

    [MenuItem("Test/Square Errors by Distance")]
    public static void TestSquareErrorsForDistance()
    {
        CalcDistErrors(SquareBound, SquareDirichlet, "Assets/Results/SquareDistErrors.txt");
    }

    [MenuItem("Test/Circle Errors by Distance")]
    public static void TestCircleErrorsForDistance()
    {
        CalcDistErrors(CircleBound, CircleDirichlet, "Assets/Results/CircleDistErrors.txt");
    }

    public static void CalcCountErrors(
        SubstituteChargeMethod.ParametricBound bound,
        SubstituteChargeMethod.ParametricDirichlet dirichlet,
        string path)
    {
        var min = 16;
        var max = 200;
        var sum = (max - min + 2) * (max - min + 1) / 2;
        var progress = 0;
        var testData = GameObject.FindObjectOfType<TestData>();
        Radius = testData.radius;
        Height = testData.height;
        var file = new StreamWriter(path, false);
        for (int i = min; i <= max; i++)
        {
            var scm = new SubstituteChargeMethod(bound, dirichlet, i, testData.distance);
            file.WriteLine("{0} {1}", i, scm.CalcMaxError(1.0 / testData.errorDelta));
            EditorUtility.DisplayProgressBar("Substitute Charge Method", i.ToString(), (float)progress / sum);
            progress += i;
        }
        EditorUtility.ClearProgressBar();
        file.Close();
    }

    public static void CalcDistErrors(
        SubstituteChargeMethod.ParametricBound bound,
        SubstituteChargeMethod.ParametricDirichlet dirichlet,
        string path)
    {
        var testData = GameObject.FindObjectOfType<TestData>();
        Radius = testData.radius;
        Height = testData.height;
        var file = new StreamWriter(path, false);
        for (float i = 1.1f; i <= 100f; i += 1f)
        {
            var scm = new SubstituteChargeMethod(bound, dirichlet, testData.count, i);
            file.WriteLine("{0} {1}", i, scm.CalcMaxError(1.0 / testData.errorDelta));
            EditorUtility.DisplayProgressBar("Substitute Charge Method", i.ToString(), i / 100f);
        }
        EditorUtility.ClearProgressBar();
        file.Close();
    }

    public static void OptimalErrors(
        SubstituteChargeMethod.ParametricBound bound,
        SubstituteChargeMethod.ParametricDirichlet dirichlet,
        string path)
    {
        var testData = GameObject.FindObjectOfType<TestData>();
        Radius = testData.radius;
        Height = testData.height;
        var file = new StreamWriter(path, false);
        for (float i = 1.1f; i <= 100f; i += 1f)
        {
            var scm = new SubstituteChargeMethod(bound, dirichlet, testData.count, i);
            file.WriteLine("{0} {1}", i, scm.CalcMaxError(1.0 / testData.errorDelta));
            EditorUtility.DisplayProgressBar("Substitute Charge Method", i.ToString(), i / 100f);
        }
        EditorUtility.ClearProgressBar();
        file.Close();
    }

    public static void TestHoge(
        SubstituteChargeMethod.ParametricBound bound,
        SubstituteChargeMethod.ParametricDirichlet dirichlet,
        Func<Vector2D, bool> isInBound)
    {
        var testData = GameObject.FindObjectOfType<TestData>();
        Radius = testData.radius;
        Height = testData.height;
        var scm = new SubstituteChargeMethod(bound, dirichlet, testData.count, testData.distance);
        testData.GetComponent<TestResult>().maxError = scm.CalcMaxError(1.0 / testData.errorDelta);

        var size = Lattice + 1;
        var center = Lattice / 2;

        var inBounds = new bool[size * size];
        var verts = new Vector3[size * size];
        var normals = new Vector3[size * size];
        var uvs = new Vector2[size * size];
        var colors = new Color[size * size];
        var unit = Radius * 2.0 / Lattice;
        var avg = 0.0;
        for (var i = 0; i < verts.Length; i++)
        {
            var pos = new Vector2D(i % size - center, i / size - center) * unit;
            var val = 0.0;
            if (isInBound(pos))
            {
                val = scm.CalcValueAt(pos);
                colors[i] = Color.clear;
                inBounds[i] = true;
            }
            colors[i] = Color.white;
            verts[i] = new Vector3((float)pos.X, (float)val, (float)pos.Y);
            normals[i] = Vector3.up;
            uvs[i] = new Vector2((float)(i % size) / Lattice, (float)(i / size) / Lattice);

            avg += val;
        }

        var indices = new List<int>(Lattice * Lattice * 2 * 3);
        var tri = new int[3];
        for (var i = 0; i < Lattice * Lattice; i++)
        {
            var x = i % Lattice;
            var y = i / Lattice;
            tri[0] = x + y * size;
            tri[1] = x + (y + 1) * size;
            tri[2] = (x + 1) + y * size;
            if (tri.All(v => inBounds[v]))
            {
                indices.AddRange(tri);
            }
            tri[0] = x + (y + 1) * size;
            tri[1] = (x + 1) + (y + 1) * size;
            tri[2] = (x + 1) + y * size;
            if (tri.All(v => inBounds[v]))
            {
                indices.AddRange(tri);
            }
        }

        var mesh = new Mesh();
        mesh.vertices = verts;
        mesh.colors = colors;
        mesh.normals = normals;
        mesh.triangles = indices.ToArray();
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var path = "Assets/Graph.asset";
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.Refresh();

        testData.graph.mesh = AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;

        while (testData.sampleRoot.childCount < scm.Samples.Length)
        {
            var inst = UnityEngine.Object.Instantiate(testData.sample) as GameObject;
            inst.transform.parent = testData.sampleRoot;
        }

        while (testData.chargeRoot.childCount < scm.Charges.Length)
        {
            var inst = UnityEngine.Object.Instantiate(testData.charge) as GameObject;
            inst.transform.parent = testData.chargeRoot;
        }

        var tUnit = 1.0 / scm.Samples.Length;
        var t = 0.0;
        for (int i = 0; i < testData.sampleRoot.childCount; i++)
        {
            var child = testData.sampleRoot.GetChild(i);
            child.gameObject.SetActive(i < scm.Samples.Length);
            if (child.gameObject.activeSelf)
            {
                var pos = new Vector3((float)scm.Samples[i].X, (float)dirichlet(t), (float)scm.Samples[i].Y);
                child.localPosition = pos;
                t += tUnit;
            }
        }
        for (int i = 0; i < testData.chargeRoot.childCount; i++)
        {
            var child = testData.chargeRoot.GetChild(i);
            child.gameObject.SetActive(i < scm.Charges.Length);
            if (child.gameObject.activeSelf)
            {
                var pos = new Vector3((float)scm.Charges[i].X, 0f, (float)scm.Charges[i].Y);
                child.localPosition = pos;
            }
        }
    }

    [MenuItem("Test/LU")]
    static void TestLU()
    {
        var mat = new double[2, 2];
        mat[0, 0] = 1;
        mat[0, 1] = 2;
        mat[1, 0] = 3;
        mat[1, 1] = -2;
        var lu = new LUDecomposition(mat, true);
        var ans = lu.SolveLinearEquations(new[]{ 1.0, -2.0 });
        Debug.Log(lu.Substitutions[0] + ", " + lu.Substitutions[1]);
        Debug.Log(ans[0] + ", " + ans[1]);
    }
}
