namespace Quagmire;

public static class Random
{
    private static System.Random Rand = new System.Random();

    public static float Float(float min = 0f, float max = 1f)
    {
        if (max < min)
        {
            (min, max) = (max, min);
        }
        return (float)Rand.NextDouble() * (max - min) + min;
    }
    
    public static int Int(int min = 0, int max = int.MaxValue)
    {
        return Rand.Next(min, max);
    }
}