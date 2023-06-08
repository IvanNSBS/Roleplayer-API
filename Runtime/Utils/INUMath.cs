namespace INUlib.Utils.Math
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
    }
}