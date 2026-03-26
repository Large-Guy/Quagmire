namespace Quagmire.Geometry;

public class Polygon
{
    public List<Point> Vertices { get; private set; }

    private List<Point> _normals;
    
    public Rectangle Bounds { get; private set; }

    public Polygon(List<Point>? points = null)
    {
        Vertices = points != null ? points : [];
        RecalculateNormals();
    }

    public void AddVertex(Point p)
    {
        Vertices.Add(p);
        RecalculateNormals();
        RecalculateBounds();
    }

    public void RemoveVertex(int i)
    {
        Vertices.RemoveAt(i);
        RecalculateNormals();
        RecalculateBounds();
    }

    private void RecalculateBounds()
    {
        var min = Vertices[0];
        var max = min;

        for (var i = 0; i < Vertices.Count; i++)
        {
            var p = Vertices[i];
            if (p.X < min.X)
                min.X = p.X;
            if (p.Y < min.Y)
                min.Y = p.Y;
            
            if (p.X > max.X)
                max.X = p.X;
            if (p.Y > max.Y)
                max.Y = p.Y;
        }

        Bounds = new Rectangle(min, max - min);
    }

    public Polygon Offset(Point offset)
    {
        var vertices = new List<Point>(Vertices.Count);
        for (var i = 0; i < Vertices.Count; i++)
        {
            vertices.Add(Vertices[i] + offset);
        }

        return new Polygon(vertices);
    }

    private void RecalculateNormals()
    {
        _normals = new List<Point>(this.Vertices.Count);
        for (var i = 0; i < Vertices.Count; i++)
        {
            var a = Vertices[i];
            var b = i == Vertices.Count - 1 ? Vertices[0] : Vertices[i + 1];
            var normal = a.Normal(b);
            _normals.Add(normal);
        }
    }

    private (float min, float max) Project(Point axis)
    {
        var min = axis.Dot(Vertices[0]);
        var max = min;

        for (var i = 1; i < Vertices.Count; i++)
        {
            var p = axis.Dot(Vertices[i]);

            if (p < min)
            {
                min = p;
            } else if (p > max)
            {
                max = p;
            }
        }
        
        return (min, max);
    }

    private static float Overlap((float min, float max) a, (float min, float max) b)
    {
        var overlapStart = Math.Max(a.min, b.min);
        var overlapEnd = Math.Min(a.max, b.max);
    
        return overlapEnd - overlapStart; 
    }
    
    public bool Intersects(Polygon other)
    {
        return MinimumTranslationVector(other) != new Point(0, 0);
    }

    public Point MinimumTranslationVector(Polygon other)
    {
        if (!Bounds.Contains(other.Bounds))
            return new Point(0, 0);
        
        float overlap = float.MaxValue;
        Point smallest = new Point(0, 0);
        
        foreach (var normal in _normals)
        {
            var thisProj = Project(normal);
            var otherProj = other.Project(normal);

            var o = Overlap(thisProj, otherProj);
            
            if (o <= 0f)
            {
                return new Point(0, 0);
            }
            
            if (o < overlap)
            {
                overlap = o;
                smallest = normal;
                
                var d = thisProj.Item1 + thisProj.Item2 - otherProj.Item1 - otherProj.Item2;
                if (d < 0)
                {
                    smallest *= -1; // Flip the normal
                } 
            }
        }
        
        foreach (var normal in other._normals)
        {
            var thisProj = Project(normal);
            var otherProj = other.Project(normal);

            var o = Overlap(thisProj, otherProj);
            
            if (o <= 0f)
            {
                return new Point(0, 0);
            }
            
            if (o < overlap)
            {
                overlap = o;
                smallest = normal;
                
                var d = thisProj.Item1 + thisProj.Item2 - otherProj.Item1 - otherProj.Item2;
                if (d < 0)
                {
                    smallest *= -1; // Flip the normal
                } 
            }
        }

        return smallest * overlap;
    }

    public static Polygon Rectangle(Point size)
    {
        return new Polygon([
            new Point(-size.X, -size.Y) * 0.5f,
            new Point(-size.X, size.Y) * 0.5f,
            new Point(size.X, size.Y) * 0.5f,
            new Point(size.X, -size.Y) * 0.5f
        ]);
    }
}