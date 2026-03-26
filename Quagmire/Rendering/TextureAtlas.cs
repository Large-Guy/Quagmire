using Quagmire.Assets;
using Quagmire.Geometry;
using SDL3;

namespace Quagmire.Rendering;

public class TextureAtlas : Texture
{
    private readonly List<Texture> _textures = [];
    private readonly List<Rectangle> _regions = [];
    private bool _dirty = true;

    public Texture this[int i] => Get(i);
    
    public TextureAtlas(string path) : base(path)
    {
        
    }

    public void Queue(Texture texture)
    {
        _textures.Add(texture);
        _dirty = true;
    }

    public int PowerOfTwo(int val)
    {
        return (int)Math.Pow(2, Math.Ceiling(Math.Log2(val)));
    }
    
    public void Build()
    {
        var maxWidth = 8096;
        var maxHeight = 8096;

        var sorted = _textures.ToList();
        sorted = sorted.OrderByDescending(texture => texture.Height).ThenByDescending(texture => texture.Width).ToList();

        var x = 0;
        var y = 0;
        var shelfHeight = 0;
        var realizedWidth = 0;
        _regions.Clear();

        foreach (var texture in sorted)
        {
            if (texture.Width > maxWidth || texture.Height > maxHeight)
                throw new Exception("Texture is too large for atlas texture");

            if (x + texture.Width > maxWidth)
            {
                x = 0;
                y += shelfHeight;
                shelfHeight = 0;
            }

            if (y + texture.Height > maxHeight)
                throw new Exception("Ran out of atlas space");

            _regions.Add(new Rectangle(x, y, texture.Width, texture.Height));

            x += texture.Width;
            shelfHeight = Math.Max(shelfHeight, texture.Height);
            realizedWidth = Math.Max(realizedWidth, x);
        }

        realizedWidth = PowerOfTwo(realizedWidth);
        var realizedHeight = PowerOfTwo(y + shelfHeight);
        
        InternalTexture = SDL.CreateTexture(Internals.Renderer, SDL.PixelFormat.RGBA8888, SDL.TextureAccess.Target,
            realizedWidth, realizedHeight);
        SDL.SetTextureScaleMode(InternalTexture, SDL.ScaleMode.PixelArt);
        
        var target = SDL.GetRenderTarget(Internals.Renderer);

        SDL.SetRenderTarget(Internals.Renderer, InternalTexture);

        SDL.SetRenderDrawColorFloat(Internals.Renderer, 0f, 0f, 0f, 0f);
        SDL.RenderClear(Internals.Renderer);

        for (var i = 0; i < _regions.Count; i++)
        {
            var region = _regions[i];
            var texture = _textures[i];
            var source = texture.GetSource();

            SDL.RenderTexture(Internals.Renderer,
                texture.GetTexture(),
                new SDL.FRect
                {
                    X = source.Position.X, Y = source.Position.Y,
                    W = source.Size.X, H = source.Size.Y
                },
                new SDL.FRect
                {
                    X = region.Position.X, Y = region.Position.Y,
                    W = region.Size.X, H = region.Size.Y
                });
        }

        SDL.SetRenderTarget(Internals.Renderer, target);
        
        Source = new Rectangle(0, 0, realizedWidth, realizedHeight);

        _dirty = false;
    }

    private TextureRegion Get(int i)
    {
        if(_dirty)
            Build();
        
        var region = _regions[i];
        return new TextureRegion(this, region);
    }
}