namespace Quagmire.Geometry;

public static class Trig
{
    public static float Radians(float degrees) => (float)(degrees * Math.PI / 180);
    public static float Degrees(float radians) => (float)(radians * 180 / Math.PI);

    public static float DifferenceDegrees(float a, float b)
    {
        var diff = ((a - b) % 360 + 540) % 360 - 180;
        return diff;
    }
}