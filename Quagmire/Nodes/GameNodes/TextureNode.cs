using Quagmire.Geometry;
using Quagmire.Rendering;

namespace Quagmire.Nodes.GameNodes;

public class TextureNode : GameNode
{
    public Texture? Texture = null;
    public Rectangle? Source = null;
    public Rectangle? Destination = null;
    public Color Tint = Color.White;

    public TextureNode(Texture? texture = null)
    {
        Texture = texture;
    }

    public override void OnDraw()
    {
        if (Texture != null)
        {
            var position = GlobalPosition + (Destination?.Position ?? new Point());
            var size = (Destination?.Size ??
                       Source?.Size ?? new Point(Texture.Width, Texture.Height) / World.Scale) * GlobalScale;
            Draw.Texture(Texture, position, size, Pivot, GlobalRotation, Tint, Source);
        }

        base.OnDraw();
    }
}