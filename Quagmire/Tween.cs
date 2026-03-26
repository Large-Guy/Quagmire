namespace Quagmire;

public static class Tween
{
    public static float Lerp(float start, float end, float t)
    {
        return start + (end - start) * t;
    }

    public static float EaseInSine(float t)
    {
        return 1 - MathF.Cos(t * MathF.PI / 2);
    }

    public static float EaseOutSine(float t)
    {
        return MathF.Sin(t * MathF.PI / 2);   
    }

    public static float EaseInOutSine(float t)
    {
        return -(MathF.Cos(MathF.PI * t) - 1) / 2;
    }

    public static float EaseInQuad(float t)
    {
        return t * t;
    }

    public static float EaseOutQuad(float t)
    {
        return 1 - (1 - t) * (1 - t);
    }
    
    public static float EaseInOutQuad(float t)
    {
        return t < 0.5 ? 2 * t * t : 1 - MathF.Pow(-2 * t + 2, 2) / 2;
    }

    public static float EaseInCubic(float t)
    {
        return t * t * t;
    }

    public static float EaseOutCubic(float t)
    {
        return 1 - MathF.Pow(1 - t, 3);
    }
    
    public static float EaseInOutCubic(float t)
    {
        return t < 0.5 ? 4 * t * t * t : 1 - MathF.Pow(-2 * t + 2, 3) / 2;
    }

    public static float EaseInQuart(float t)
    {
        return t * t * t * t;
    }

    public static float EaseOutQuart(float t)
    {
        return 1 - MathF.Pow(1 - t, 4);
    }
    
    public static float EaseInOutQuart(float t)
    {
        return t < 0.5 ? 8 * t * t * t * t : 1 - MathF.Pow(-2 * t + 2, 4) / 2;
    }

    public static float EaseInQuint(float t)
    {
        return t * t * t * t * t;
    }

    public static float EaseOutQuint(float t)
    {
        return 1 - MathF.Pow(1 - t, 5);
    }
    
    public static float EaseInOutQuint(float t)
    {
        return t < 0.5 ? 16 * t * t * t * t * t : 1 - MathF.Pow(-2 * t + 2, 5) / 2;
    }

    public static float EaseInExpo(float t)
    {
        return t == 0 ? 0 : MathF.Pow(2, 10 * t - 10);
    }

    public static float EaseOutExpo(float t)
    {
        return t == 1 ? 1 : 1 - MathF.Pow(2, -10 * t);
    }
    
    public static float EaseInOutExpo(float t)
    {
        return t == 0 ? 0 : t == 1 ? 1 : t < 0.5 ? MathF.Pow(2, 20 * t - 10) / 2 : (2 - MathF.Pow(2, -20 * t + 10)) / 2;
    }

    public static float EaseInCirc(float t)
    {
        return 1 - MathF.Sqrt(1 - t * t);
    }
    
    public static float EaseOutCirc(float t)
    {
        return MathF.Sqrt(1 - MathF.Pow(t - 1, 2));
    }
    
    public static float EaseInOutCirc(float t)
    {
        return t < 0.5 ? (1 - MathF.Sqrt(1 - t * t)) / 2 : (MathF.Sqrt(1 - MathF.Pow(-2 * t + 2, 2)) + 1) / 2;
    }

    public static float EaseInBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;
        
        return c3 * t * t * t - c1 * t * t;
    }

    public static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;
        
        return 1 + c3 * MathF.Pow(t - 1, 3) + c1 * MathF.Pow(t - 1, 2);
    }

    public static float EaseInOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;
        
        return t < 0.5 ? (MathF.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2 : (MathF.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;   
    }

    public static float EaseInElastic(float t)
    {
        const float c4 = (2 * MathF.PI) / 3;
        return t == 0 ? 0 : t == 1 ? 1 : -MathF.Pow(2, 10 * t - 10) * MathF.Sin((t * 10 - 10.75f) * c4);
    }
    
    public static float EaseOutElastic(float t)
    {
        const float c4 = (2 * MathF.PI) / 3;
        return t == 0 ? 0 : t == 1 ? 1 : MathF.Pow(2, -10 * t) * MathF.Sin((t * 10 - 0.75f) * c4) + 1;
    }

    public static float EaseInOutElastic(float t)
    {
        const float c4 = (2 * MathF.PI) / 3;
        return t == 0 ? 0 :
            t == 1 ? 1 :
            t < 0.5 ? -(MathF.Pow(2, 20 * t - 10) * MathF.Sin((20 * t - 11.125f) * c4)) / 2 :
            (MathF.Pow(2, -20 * t + 10) * MathF.Sin((20 * t - 11.125f) * c4)) / 2 + 1;
    }
}