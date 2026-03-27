using System.Numerics;
using System.Runtime.InteropServices;
using Quagmire.Geometry;
using Quagmire.Rendering.Commands;
using SDL3;

namespace Quagmire.Rendering;

public static class Draw
{
    public static IntPtr Renderer;
    private static IntPtr _targetTexture;
    private static int _width, _height;

    public static Point Size => new Point(_width, _height);
    public static Camera? Camera = null;


    private static Dictionary<Type, IGenericPool> _callPools = [];
    private static List<IDrawCommand> _drawCommands = [];

    public static float Depth = 0f;

    private static T Command<T>() where T : class, new()
    {
        if (!_callPools.ContainsKey(typeof(T)))
        {
            _callPools[typeof(T)] = new Pool<T>();
        }
        return ((Pool<T>)_callPools[typeof(T)]).New;
    }

    private static void Return(IDrawCommand item)
    {
        var type = item.GetType();
        
        if (!_callPools.TryGetValue(type, out var pool))
        {
            var poolType = typeof(Pool<>).MakeGenericType(type);
            pool = (IGenericPool)Activator.CreateInstance(poolType)!;
            _callPools[type] = pool;
        }

        pool.Delete(item);
    }

    public static void Resolution(int width, int height)
    {
        _width = width;
        _height = height;
        _targetTexture = SDL.CreateTexture(Internals.Renderer, SDL.PixelFormat.RGBA8888, SDL.TextureAccess.Target, _width, _height);
        SDL.SetTextureScaleMode(_targetTexture, SDL.ScaleMode.Nearest);
    }

    public static void Flush()
    {
        Internals.NewFrame();
        
        SDL.SetRenderTarget(Internals.Renderer, _targetTexture);
        SDL.SetRenderDrawColorFloat(Internals.Renderer, World.Background.R, World.Background.G, World.Background.B,
            World.Background.A);
        SDL.RenderClear(Internals.Renderer);
        
        //flush draw commands
        _drawCommands.Sort((a, b) => a.ZIndex.CompareTo(b.ZIndex));

        SDL.SetRenderDrawBlendMode(Internals.Renderer, SDL.BlendMode.Blend);
        
        foreach (var t in _drawCommands)
        {
            t.Draw();
            dynamic generic = t;
            Return(generic);
        }

        SDL.SetRenderTarget(Internals.Renderer, IntPtr.Zero);

        SDL.RenderTexture(Internals.Renderer, _targetTexture, IntPtr.Zero, IntPtr.Zero);

        SDL.RenderPresent(Internals.Renderer);
        
        _drawCommands.Clear();
    }

    private static Vector2 ConvertPoint(Point point)
    {
        var floored = point.Floor();
        return new Vector2(floored.X, floored.Y);
    }

    public static Point ToScreen(Point p, bool floor = true)
    {
        var position = p * World.Scale;
        var cameraPosition = (Camera?.Position ?? new Point()) * World.Scale;
        var pivotOffset = Camera?.Pivot * Size ?? new Point();
        var offset = position + cameraPosition + pivotOffset;
        return floor ? offset.Floor() : offset;
    }

    private static Geometry.Rectangle ToScreenR(Geometry.Rectangle rect)
    {
        return new Geometry.Rectangle(ToScreen(rect.Position), (rect.Size * World.Scale).Floor());
    }

    public static void Rectangle(Color color, Geometry.Rectangle rectangle, Point? origin = null, float rotation = 0f)
    {
        var rect = ToScreenR(rectangle);
        var item = Command<RectangleCommand>();
        item.Color = color;
        item.Rectangle = rect.Floor();
        item.Origin = origin ?? new Point();
        item.Rotation = rotation;
        item.ZIndex = Depth;
        _drawCommands.Add(item);
    }

    public static void ConvexPolygon(Color color, Polygon polygon)
    {
        if (polygon.Vertices.Count < 3)
            return;

        for (var i = 1; i < polygon.Vertices.Count - 1; i++)
        {
            var item = Command<TriangleCommand>();
            item.A = ToScreen(polygon.Vertices[0]);
            item.B = ToScreen(polygon.Vertices[i]);
            item.C = ToScreen(polygon.Vertices[i + 1]);
            item.Color = color;
            item.ZIndex = Depth;
            _drawCommands.Add(item);
        }
    }

    public static void Triangle(Color color, Point a, Point b, Point c)
    {
        var item = Command<TriangleCommand>();
        item.A = ToScreen(a);
        item.B = ToScreen(b);
        item.C = ToScreen(c);
        item.Color = color;
        item.ZIndex = Depth;
        _drawCommands.Add(item);
    }

    public static void Point(Color color, Point point)
    {
        var item = Command<PointCommand>();
        item.Color = color;
        item.Point = ToScreen(point);
        item.ZIndex = Depth;
        _drawCommands.Add(item);
    }

    public static void Line(Color color, Point a, Point b, float width = 1f)
    {
        var item = Command<LineCommand>();
        item.A = ToScreen(a);
        item.B = ToScreen(b);
        item.Color = color;
        item.Width = width;
        item.ZIndex = Depth;
        _drawCommands.Add(item);
    }

    public static void Bezier(Color color, Point a, Point b, Point control, float width = 1f)
    {
        const int detail = 32;
        var prev = a;
        for (var i = 1; i < detail; i++)
        {
            var t = (float)i / detail;
            var p = (a.Lerp(control, t)).Lerp(control.Lerp(b, t), t);
            Line(color, prev, p, width);
            prev = p;
        }
    }

    public static void Circle(Color color, Point center, float radius)
    {
        var item = Command<CircleCommand>();
        item.Center = ToScreen(center, false);
        item.Color = color;
        item.Fill = true;
        item.Radius = MathF.Floor(radius * World.Scale.X);
        item.Arc = MathF.Tau;
        item.Origin = 0f;
        item.ZIndex = Depth;
        
        _drawCommands.Add(item);
    }

    public static void Sector(Color color, Point center, float radius, float arc, float origin)
    {
        var item = Command<CircleCommand>();
        item.Center = ToScreen(center, false);
        item.Color = color;
        item.Fill = true;
        item.Radius = MathF.Floor(radius * World.Scale.X);
        item.Arc = arc;
        item.Origin = origin;
        item.ZIndex = Depth;
        
        _drawCommands.Add(item);
    }
    
    public static void CircleOutline(Color color, Point center, float radius, float width = 1f)
    {
        var item = Command<CircleCommand>();
        item.Center = ToScreen(center);
        item.Color = color;
        item.Fill = false;
        item.Radius = MathF.Floor(radius * World.Scale.X);
        item.ZIndex = Depth;
        item.Arc = MathF.Tau;
        item.Origin = 0f;
        _drawCommands.Add(item);
    }

    public static void Ray(Color color, Point start, Point direction, float length = 16f)
    {
        Point end = start + direction.Normalize() * length;
        Line(color, start, end);
    }

    public static void Plane(Color color, Point start, Point direction, float length = 16f)
    {
        var a = start + direction.Normalize() * -length;
        var b = start + direction.Normalize() * length;
        Line(color, a, b);
    }

    public static void Texture(Texture texture, Point position, Point size, Point origin, float rotation,
        Color? tint = null, Geometry.Rectangle? src = null)
    {
        
        var sourcePosition = new Point(texture.GetSource().Position.X, texture.GetSource().Position.Y) +
                             (src?.Position ?? new Point());
        var sourceSize = src?.Size ?? new Point(texture.GetSource().Size.X, texture.GetSource().Size.Y);

        var item = Command<TextureCommand>();
        item.Origin = origin;
        item.Position = ToScreen(position);
        item.Rotation = rotation;
        item.Size = (size * World.Scale).Floor();
        item.Src = new Geometry.Rectangle(sourcePosition, sourceSize);
        item.Texture = texture;
        item.Tint = tint ?? Color.White;
        item.ZIndex = Depth;
        
        _drawCommands.Add(item);
    }

    public static void NinePatch(NinePatch patch, Point position, Point size, Color? tint = null)
    {
        var patchCommand = Command<NinePatchCommand>();
        patchCommand.Tint = tint ?? Color.White;
        patchCommand.Position = position.Floor();
        patchCommand.Size = (size * World.Scale).Floor();
        patchCommand.Texture = patch;
        patchCommand.ZIndex = Depth;
        
        _drawCommands.Add(patchCommand);
    }

    public static void Text(string text, Font font, float size, Point position, Color color, int wrapLength = 0)
    {
        var textCommand = Command<TextCommand>();
        textCommand.Color = color;
        textCommand.Font = font;
        textCommand.Position = ToScreen(position);
        textCommand.Size = size;
        textCommand.Text = text;
        textCommand.ZIndex = Depth;
        textCommand.WrapLength = wrapLength;

        _drawCommands.Add(textCommand);
    }
}