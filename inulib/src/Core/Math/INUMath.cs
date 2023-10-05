using System;

namespace INUlib.Core.Math
{
    public static class INUMath
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        
        public static int Clamp01(int value) => Clamp(value, 0, 1);
        public static float Clamp01(float value) => Clamp(value, 0, 1);

        public static float2 RotateDegrees(this float2 target, float degAngle)
        {
            float cos = MathF.Cos(degAngle);
            float sin = MathF.Sin(degAngle);
            return new float2(cos * target.x - sin * target.y, sin * target.x + cos * target.y);
        }
    }
}