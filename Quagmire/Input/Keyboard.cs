using Quagmire.Geometry;
using SDL3;

namespace Quagmire.Input;

public static class Keyboard
{
    public static bool IsKeyJustPressed(Key key)
    {
        if(!Internals.KeysPressed.ContainsKey((SDL.Keycode)key))
            return false;
        var keyState = Internals.KeysPressed[(SDL.Keycode)key];
        return keyState == Internals.ButtonState.JustPressed;
    }
    
    public static bool IsKeyPressed(Key key)
    {
        if(!Internals.KeysPressed.ContainsKey((SDL.Keycode)key))
            return false;
        var keyState = Internals.KeysPressed[(SDL.Keycode)key];
        return keyState == Internals.ButtonState.JustPressed || keyState == Internals.ButtonState.Pressed;
    }

    public static bool IsKeyJustReleased(Key key)
    {
        if(!Internals.KeysPressed.ContainsKey((SDL.Keycode)key))
            return false;
        var keyState = Internals.KeysPressed[(SDL.Keycode)key];
        return keyState == Internals.ButtonState.JustReleased;
    }

    public static bool IsKeyReleased(Key key)
    {
        if(!Internals.KeysPressed.ContainsKey((SDL.Keycode)key))
            return false;
        var keyState = Internals.KeysPressed[(SDL.Keycode)key];
        return keyState == Internals.ButtonState.JustReleased || keyState == Internals.ButtonState.Released;
    }

    public static float Axis(Key negative, Key positive)
    {
        var v = IsKeyPressed(positive) ? 1f : 0f;
        v -= IsKeyPressed(negative) ? 1f : 0f;
        return v;
    }

    public static Point Vector(Key negativeX, Key positiveX, Key negativeY, Key positiveY, bool normalize = true)
    {
        var p = new Point(Axis(negativeX, positiveX), Axis(negativeY, positiveY));
        if (p.Length() > 1)
            p = p.Normalize();
        return p;
    }
}