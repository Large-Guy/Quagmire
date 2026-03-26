using Quagmire.Assets;
using Quagmire.Geometry;

namespace Quagmire.Rendering;

public class NinePatch : Asset
{
    public Texture Texture;
    public Rectangle Center;
    
    public NinePatch(Texture texture, Rectangle center) : base("")
    {
        Texture = texture;
        Center = center;
    }
}