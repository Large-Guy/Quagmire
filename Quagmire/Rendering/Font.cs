using Quagmire.Assets;
using Quagmire.Geometry;
using SDL3;

namespace Quagmire.Rendering;

public class Font : Asset
{
    public string File;
    public Dictionary<float, IntPtr> Sizes = [];
    internal static IntPtr TextEngine;

    public Font(string file) : base(file)
    {
        File = file;
        TextEngine = TTF.CreateRendererTextEngine(Draw.Renderer);
    }

    internal IntPtr GetFont(float size)
    {
        if (Sizes.ContainsKey(size))
            return Sizes[size];
        var font = TTF.OpenFont(File, (int)size);
        Sizes.Add(size, font);
        return font;
    }
}