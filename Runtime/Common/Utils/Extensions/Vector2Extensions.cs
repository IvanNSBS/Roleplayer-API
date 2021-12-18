using UnityEngine;

namespace Common.Utils.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector2 RotateDegrees(this Vector2 target, float degAngle)
        {
            float cos = Mathf.Cos(degAngle);
            float sin = Mathf.Sin(degAngle);
            return new Vector2(cos * target.x - sin * target.y, sin * target.x + cos * target.y);
        }
    }
}