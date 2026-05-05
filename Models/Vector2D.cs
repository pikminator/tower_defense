namespace tower_defense.Models;

public readonly struct Vector2D(double x, double y)
{
    public double X { get; } = x;
    public double Y { get; } = y;

    public double DistanceTo(Vector2D other)
    {
        double dx = X - other.X;
        double dy = Y - other.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public double LengthSquared() => X * X + Y * Y;

    public static Vector2D operator +(Vector2D a, Vector2D b) => new(a.X + b.X, a.Y + b.Y);
    public static Vector2D operator -(Vector2D a, Vector2D b) => new(a.X - b.X, a.Y - b.Y);
    public static Vector2D operator *(Vector2D v, double s) => new(v.X * s, v.Y * s);
    public static Vector2D operator *(double s, Vector2D v) => new(v.X * s, v.Y * s);
}
