namespace Quagmire.Rendering;

public struct Color
{
    public static readonly Color Black = new Color();
    public static readonly Color Red = new Color(1f, 0f, 0f);
    public static readonly Color Green = new Color(0f, 1f, 0f);
    public static readonly Color Yellow = new Color(1f, 1f, 0f);
    public static readonly Color Blue = new Color(0f, 0f, 1f);
    public static readonly Color Magenta = new Color(1f, 0f, 1f);
    public static readonly Color White = new Color(1f);
    
    
    public float R, G, B, A;

    public Color()
    {
        R = 0f;
        G = 0f;
        B = 0f;
        A = 1f;
    }
    
    public Color(float v, float a = 1f)
    {
        R = v;
        G = v;
        B = v;
        A = a;
    }

    public Color(float r, float g, float b, float a = 1f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(byte v, byte a = 255)
    {
        var fv = v / 255f;
        R = fv;
        G = fv;
        B = fv;
        A = a / 255f;
    }

    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r / 255f;
        G = g / 255f;
        B = b / 255f;
        A = a / 255f;
    }

    public Color Mix(Color other, float amount)
    {
        return new Color()
        {
            R = (other.R - R) * amount + R,
            G = (other.G - G) * amount + G,
            B = (other.B - B) * amount + B,
            A = (other.A - A) * amount + A
        };
    }
}