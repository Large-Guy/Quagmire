using Quagmire.Geometry;
using SDL3;

namespace Quagmire.Rendering.Commands;

public class TextCommand : IDrawCommand
{
    public float ZIndex { get; set; }
    public Color Color;
    public Font? Font;
    public float Size;
    public string Text = "";
    public Point Position;
    public int WrapLength;
    public void Draw()
    {
        if(Font == null)
            throw new Exception("Font cannot be null");
        var font = Font.GetFont(Size);
        var obj = TTF.CreateText(Font.TextEngine, font, Text, (UIntPtr)Text.Length);
        TTF.SetTextColorFloat(obj, Color.R, Color.G, Color.B, Color.A);
        if (WrapLength != 0)
        {
            TTF.SetTextWrapWidth(obj, WrapLength);
        }
        TTF.DrawRendererText(obj, Position.X, Position.Y);
        TTF.DestroyText(obj);
    }
}