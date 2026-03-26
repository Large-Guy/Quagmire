
namespace Quagmire;

public static class Time
{
    public static float TimeScale = 1f;
    public static float RunTime => Internals.RunTime;
    public static float FrameRate => Internals.FrameRate;
    public static float DeltaTime => Math.Clamp(Internals.DeltaTime, 0f, 0.1f) * TimeScale;
}