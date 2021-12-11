using System;
using UnityEngine;

namespace INUlib.RPG.AI.Movement.Steering.Behaviour
{
    public static class SteerDirection
    {
        private static Vector2 RotateVector2(float angle, Vector3 target)
        {
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
            return new Vector2(cos * target.x - sin * target.y, sin * target.x + cos * target.y);
        }

        public static Vector3 Avoid(Vector3 from, Vector3 desiredVel, float radius, int rayNumber, int collisionMask, bool debug=false)
        {
            float increment = 360f /(float)rayNumber;
            increment *= Mathf.Deg2Rad;

            float desiredLenght = desiredVel.magnitude;
            Vector3 result = desiredVel;
            int hits = 1;
            float length = radius;

            for (int i = 0; i < rayNumber; i++)
            {
                Vector3 direction = RotateVector2(i * increment, Vector3.right);
                RaycastHit2D hit = Physics2D.Raycast(from, direction, radius, ~(1 << 7));

                if(hit.collider)
                {
                    length = Vector3.Distance(hit.point, from);

                    float normalized = length / radius;
                    float normalizedDistance = 1 - normalized*normalized;
                    float finalLength = desiredLenght * normalizedDistance;

                    Vector3 desiredRay = -direction * finalLength; // dont need to normalize, Vector3.right is a unit vector
                    result += desiredRay;
                    hits++;
                }

                #if UNITY_EDITOR
                if(debug)
                {
                    Gizmos.color = hit.collider ? Color.red : Color.white;
                    Gizmos.DrawLine(from, from + direction*length);
                }
                #endif
            }

            return result.normalized * desiredLenght;
        }

        public static float ArriveFactor(Vector3 from, Vector3 at, float acceptDst, float scalingFactor)
        {
            float distance = Vector3.Distance(from, at);
            float acceptTwo = acceptDst * 2;

            float factor = 1;
            if(distance <= acceptTwo)
            {
                float dst = distance - acceptDst;
                factor = dst / acceptDst;
                factor = Mathf.Clamp01(factor*scalingFactor);
            }

            return factor;
        }
    }
}