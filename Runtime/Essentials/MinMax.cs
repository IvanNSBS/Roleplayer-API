using UnityEngine;

namespace Essentials
{
    public class MinMax : PropertyAttribute
    {
        public float min;
        public float max;

        public MinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}