using Quagmire.Assets;
using Quagmire.Geometry;
using SDL3;

namespace Quagmire.Rendering;

public class Texture : Asset
{
    private static Dictionary<IntPtr, int> _referenceCounting = [];
    private IntPtr _textureReference = IntPtr.Zero;
    protected IntPtr InternalTexture {
        get => _textureReference;
        set
        {
            if (_textureReference != IntPtr.Zero)
            {
                if (!_referenceCounting.ContainsKey(_textureReference))
                {
                    Console.WriteLine($"Possibly leaked texture memory on reference {_textureReference}");
                }
                _referenceCounting[_textureReference]--;
                if (_referenceCounting[_textureReference] == 0)
                {
                    _referenceCounting.Remove(_textureReference);
                    SDL.DestroyTexture(_textureReference);
                }
            }
            
            _textureReference = value;
            if (!_referenceCounting.TryAdd(value, 1))
            {
                _referenceCounting[_textureReference]++;
            }
        }
    }
    protected Rectangle Source;

    public int Width => (int)Source.Size.X;
    public int Height => (int)Source.Size.Y;
    public Point Size => new Point(Width, Height);
    
    public Texture(string path = "") : base(path)
    {
    }

    ~Texture()
    {
        InternalTexture = IntPtr.Zero;
    }

    public virtual Rectangle GetSource()
    {
        return Source;
    }

    public virtual IntPtr GetTexture()
    {
        return InternalTexture;
    }
}