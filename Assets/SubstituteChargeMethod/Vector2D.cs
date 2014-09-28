using System;

[System.Serializable]
public struct Vector2D
{
    public double X;
    public double Y;

    public Vector2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double Length
    {
        get
        {
            return Math.Sqrt(Dot(this, this));
        }
    }

    public Vector2D Normalized
    {
        get
        {
            return this / Length;
        }
    }

    public static double Dot(Vector2D a, Vector2D b)
    {
        return a.X * b.X + a.Y * b.Y;
    }

    public static Vector2D operator +(Vector2D a, Vector2D b)
    {
        return new Vector2D(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2D operator -(Vector2D a, Vector2D b)
    {
        return new Vector2D(a.X - b.X, a.Y - b.Y);
    }

    public static Vector2D operator *(Vector2D a, double b)
    {
        return new Vector2D(a.X * b, a.Y * b);
    }

    public static Vector2D operator *(double a, Vector2D b)
    {
        return new Vector2D(a * b.X, a * b.Y);
    }

    public static Vector2D operator /(Vector2D a, double b)
    {
        return new Vector2D(a.X / b, a.Y / b);
    }
}
