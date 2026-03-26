namespace Quagmire.Geometry;

public struct Rectangle
{
    public Point Position, Size;

    public float Area => Size.X * Size.Y;

    public Rectangle(float x, float y, float w, float h)
    {
        Position = new Point(x, y);
        Size = new Point(w, h);
    }
    
    public Rectangle(Point position, Point size)
    {
        Position = position;
        Size = size;
    }

    public static Rectangle operator *(Rectangle a, float scalar)
    {
        return new Rectangle(a.Position * scalar, a.Size * scalar);
    }
    
    public static Rectangle operator *(Rectangle a, Point scalar)
    {
        return new Rectangle(a.Position * scalar, a.Size * scalar);
    }

    public static Rectangle operator /(Rectangle a, float scalar)
    {
        return new Rectangle(a.Position / scalar, a.Size / scalar);
    }
    
    public static Rectangle operator /(Rectangle a, Point scalar)
    {
        return new Rectangle(a.Position / scalar, a.Size / scalar);
    }
    
    public bool Contains(Point p)
    {
        return Position.X <= p.X && p.X <= Position.X + Size.X && Position.Y <= p.Y && p.Y <= Position.Y + Size.Y;
    }

    public bool Contains(Rectangle other)
    {
        return Position.X <= other.Position.X + other.Size.X && other.Position.X <= Position.X + Size.X &&
               Position.Y <= other.Position.Y + other.Size.Y && other.Position.Y <= Position.Y + Size.Y;
    }

    public Rectangle Floor()
    {
        return new Rectangle(Position.Floor(), Size.Floor());
    }

    public Rectangle Ceil()
    {
        return new Rectangle(Position.Ceil(), Size.Ceil());
    }
    
    public override string ToString()
    {
        return $"(X: {Position.X}, Y: {Position.Y}, W: {Size.X}, H: {Size.Y})";
    }
}