using System;

namespace INUlib.Tweening
{
    public static class EasingFunctions
    {
        #region Methods
        public static float Linear(float a, float b, float t)
        {
            return Lerp(a, b, t);
        }

        public static float InSine(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float OutSine(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InOutSine(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InQuad(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float OutQuad(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InOutQuad(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InCubic(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float OutCubic(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InOutCubic(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InQuart(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float OutQuart(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InOutQuart(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InQuint(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float OutQuint(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InOutQuint(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InExpo(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float OutExpo(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InOutExpo(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InCirc(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float OutCirc(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InOutCirc(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InBack(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float OutBack(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InOutBack(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InElastic(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float OutElastic(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InOutElastic(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InBounce(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float OutBounce(float a, float b, float t)
        {
            throw new NotImplementedException();
        }

        public static float InOutBounce(float a, float b, float t)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Helper Methods
        private static float Lerp(float a, float b, float t) => (1 - t) * a + t * b;
        #endregion
    }
}