using SDL3;

namespace Quagmire.Rendering;

public static class Window
{
    public static int Width
    {
        get
        {
            SDL.GetWindowSize(Internals.Window, out var w, out var _);
            return w;
        }
    }

    public static int Height {
        get
        {
            SDL.GetWindowSize(Internals.Window, out var _, out var h);
            return h;
        }
    }

    public static bool ShouldClose;
    
    public static void Open(string name, int width, int height)
    {
        if (SDL.WasInit(SDL.InitFlags.Video) == 0)
        {
            if (!SDL.Init(SDL.InitFlags.Video) || !TTF.Init())
            {
                throw new Exception(SDL.GetError());
            }
        }

        var displayScale = SDL.GetDisplayContentScale(SDL.GetPrimaryDisplay());
        Console.WriteLine($"Display Scale: {displayScale}");
        var scaledWidth = width / displayScale;
        var scaledHeight = height / displayScale;
        SDL.CreateWindowAndRenderer(name, (int)scaledWidth, (int)scaledHeight, SDL.WindowFlags.HighPixelDensity, out Internals.Window, out Internals.Renderer);
        SDL.SetDefaultTextureScaleMode(Internals.Renderer, SDL.ScaleMode.PixelArt);
        SDL.StartTextInput(Internals.Window);
        Draw.Resolution(width, height);
    }
    
    public static void Close()
    {
        ShouldClose = true;
        SDL.DestroyWindow(Internals.Window);
        SDL.DestroyRenderer(Internals.Renderer);
    }

    public static bool IsOpen()
    {
        return !ShouldClose;
    }
}