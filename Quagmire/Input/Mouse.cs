using Quagmire.Geometry;
using Quagmire.Rendering;
using SDL3;

namespace Quagmire.Input;

public static class Mouse
{
    public static Point Position
    {
        get
        {
            SDL.GetMouseState(out var x, out var y);
            return new Point(x, y) * new Point(Draw.Size.X / Window.Width, Draw.Size.Y / Window.Height);
        }
    }

    public static Point Delta => Internals.MouseDelta;

    public static bool IsButtonJustPressed(Button button)
    {
        if(!Internals.MouseState.ContainsKey((SDL.MouseButtonFlags)button))
            return false;
        var keyState = Internals.MouseState[(SDL.MouseButtonFlags)button];
        return keyState == Internals.ButtonState.JustPressed;
    }

    public static bool IsButtonPressed(Button button)
    {
        if(!Internals.MouseState.ContainsKey((SDL.MouseButtonFlags)button))
            return false;
        var keyState = Internals.MouseState[(SDL.MouseButtonFlags)button];
        return keyState == Internals.ButtonState.JustPressed || keyState == Internals.ButtonState.Pressed;
    }
    
    public static bool IsButtonJustReleased(Button button)
    {
        if(!Internals.MouseState.ContainsKey((SDL.MouseButtonFlags)button))
            return false;
        var keyState = Internals.MouseState[(SDL.MouseButtonFlags)button];
        return keyState == Internals.ButtonState.JustReleased;
    }

    public static bool IsButtonReleased(Button button)
    {
        if(!Internals.MouseState.ContainsKey((SDL.MouseButtonFlags)button))
            return false;
        var keyState = Internals.MouseState[(SDL.MouseButtonFlags)button];
        return keyState == Internals.ButtonState.JustReleased || keyState == Internals.ButtonState.Released;
    }
}