using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Quagmire.Geometry;
using SDL3;

namespace Quagmire;

internal static class Internals
{
    public static float RunTime;
    public static float DeltaTime;
    public static float FrameRate;
    private static ulong _previousTicks;
    public static IntPtr Window;
    public static IntPtr Renderer;
    public static Point MouseDelta;

    public enum ButtonState
    {
        JustPressed = 0,
        Pressed = 1,
        JustReleased = 2,
        Released = 3,
    }

    public static Dictionary<SDL.Keycode, ButtonState> KeysPressed = new();
    public static Dictionary<SDL.MouseButtonFlags, ButtonState> MouseState = new();

    public static SDL.Keycode Pressed = SDL.Keycode.Unknown;
    public static string Text = "";

    public static void KeyPress(SDL.Keycode key)
    {
        KeysPressed.TryAdd(key, ButtonState.Released);
        
        if (KeysPressed[key] == ButtonState.Released || KeysPressed[key] == ButtonState.JustReleased)
        {
            KeysPressed[key] = ButtonState.JustPressed;
        }
    }

    public static void KeyRelease(SDL.Keycode key)
    {
        KeysPressed.TryAdd(key, ButtonState.Pressed);
       
        if (KeysPressed[key] == ButtonState.Pressed || KeysPressed[key] == ButtonState.JustPressed)
        {
            KeysPressed[key] = ButtonState.JustReleased;
        }
    }

    public static void MousePress(SDL.MouseButtonFlags button)
    {
        MouseState.TryAdd(button, ButtonState.Released);
        if (MouseState[button] == ButtonState.Released || MouseState[button] == ButtonState.JustReleased)
        {
            MouseState[button] = ButtonState.JustPressed;
        }
    }

    public static void MouseRelease(SDL.MouseButtonFlags button)
    {
        MouseState.TryAdd(button, ButtonState.Pressed);
        
        if (MouseState[button] == ButtonState.Pressed || MouseState[button] == ButtonState.JustPressed)
        {
            MouseState[button] = ButtonState.JustReleased;
        }
    }
    
    public static void NewFrame()
    {
        var ticks = SDL.GetTicks();
        DeltaTime = (ticks - _previousTicks) * 0.001f;
        _previousTicks = ticks;
        FrameRate = 1.0f / DeltaTime;
        RunTime += DeltaTime;

        foreach (var key in KeysPressed)
        {
            KeysPressed[key.Key] = key.Value switch
            {
                ButtonState.JustPressed => ButtonState.Pressed,
                ButtonState.JustReleased => ButtonState.Released,
                _ => KeysPressed[key.Key]
            };
        }

        foreach (var button in MouseState)
        {
            MouseState[button.Key] = button.Value switch
            {
                ButtonState.JustPressed => ButtonState.Pressed,
                ButtonState.JustReleased => ButtonState.Released,
                _ => MouseState[button.Key]
            };
        }
        
        MouseDelta = new Point(0, 0);
        
        Pressed = SDL.Keycode.Unknown;
        Text = "";
        
        
        while (SDL.PollEvent(out var e))
        {
            switch ((SDL.EventType)e.Type)
            {
                case SDL.EventType.Quit:
                {
                    Rendering.Window.Close();
                    break;
                }
                case SDL.EventType.KeyDown:
                {
                    KeyPress(e.Key.Key);
                    Pressed = e.Key.Key;
                    break;
                }
                case SDL.EventType.TextInput:
                {
                    string? text = Marshal.PtrToStringUTF8(e.Text.Text);
                    if (text != null && text.Length > 0)
                    {
                        Text = text;
                    }
                    break;
                }
                case SDL.EventType.KeyUp:
                {
                    KeyRelease(e.Key.Key);
                    break;
                }
                case SDL.EventType.MouseButtonDown:
                {
                    MousePress((SDL.MouseButtonFlags)e.Button.Button);
                    break;
                }
                case SDL.EventType.MouseButtonUp:
                {
                    MouseRelease((SDL.MouseButtonFlags)e.Button.Button);
                    break;
                }
                case SDL.EventType.MouseMotion:
                {
                    MouseDelta = new Point(e.Motion.XRel, e.Motion.YRel);
                    break;
                }
            }
        }
    }
}