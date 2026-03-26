namespace Quagmire.Rendering;

public interface IDrawCommand
{
    public float ZIndex { get; set; }
    public void Draw();
}