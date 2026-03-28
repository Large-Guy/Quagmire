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

    public static void Text(ref string output, ref int cursor)
    {
        var pressed = Internals.Pressed;

        switch (pressed)
        {
            case SDL.Keycode.Backspace:
            {
                if (cursor > 0)
                {
                    output = output.Remove(cursor - 1, 1);
                    cursor--;
                }
                break;
            }
            case SDL.Keycode.Left:
            {
                if (cursor > 0)
                    cursor--;

                break;
            }
            case SDL.Keycode.Right:
            {
                if(cursor < output.Length)
                    cursor++;
                break;
            }
            default:
            {
                if (Internals.Text.Length > 0)
                {
                    output = output.Insert(cursor, Internals.Text);
                    cursor += Internals.Text.Length;
                }
                break;
            }
        }
    }
}