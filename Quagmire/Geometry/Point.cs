namespace Quagmire.Geometry;

public struct Point
{
    public static readonly Point Zero = new Point(0, 0);

    public float X, Y;

    public Point(float v = 0f)
    {
        X = v;
        Y = v;
    }

    public Point(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }
    
    public static Point operator -(Point a, Point b)
    {
        return new Point(a.X - b.X, a.Y - b.Y);
    }
    
    public static Point operator *(Point a, Point b)
    {
        return new Point(a.X * b.X, a.Y * b.Y);
    }

    public static Point operator *(Point a, float scalar)
    {
        return new Point(a.X * scalar, a.Y * scalar);
    }

    public static Point operator /(Point a, Point b)
    {
        return new Point(a.X / b.X, a.Y / b.Y);
    }
    
    public static Point operator /(Point a, float scalar)
    {
        return new Point(a.X / scalar, a.Y / scalar);
    }

    public static bool operator ==(Point a, Point b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(Point a, Point b)
    {
        return !(a == b);
    }

    public Point Perpendicular()
    {
        return new Point(-Y, X);
    }

    public float Dot(Point other)
    {
        return X * other.X + Y * other.Y;
    }

    public Point Normal(Point other)
    {
        return (other - this).Perpendicular().Normalize();
    }

    public Point Lerp(Point other, float t)
    {
        return (other - this) * t + this;
    }

    public Point Midpoint(Point other)
    {
        return Lerp(other, 0.5f);
    }

    public float Length()
    {
        return MathF.Sqrt(X * X + Y * Y);
    }

    public float Distance(Point other)
    {
        return (other - this).Length();
    }

    public Point Direction(Point other)
    {
        return (other - this).Normalize();
    }

    public Point Normalize()
    {
        if (Length() < 0.00001f)
            return new Point();
        return this / Length();
    }

    public Point Floor()
    {
        return new Point(MathF.Floor(X), MathF.Floor(Y));
    }

    public Point Ceil()
    {
        return new Point(MathF.Ceiling(X), MathF.Ceiling(Y));
    }

    public float Angle(Point p)
    {
        var dir = this.Direction(p);
        return MathF.Atan2(dir.Y, dir.X);
    }

    public Point ClampLength(float length)
    {
        if (this.Length() > length)
            return new Point().Direction(this) * length;
        return this;
    }

    public override string ToString()
    {
        return $"(X: {X}, Y: {Y})";
    }
}