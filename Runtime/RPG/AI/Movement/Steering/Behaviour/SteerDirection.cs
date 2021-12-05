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

        public static Vector3 AverageVector(Vector3 from, Vector3 desiredDir, float rayLength, int rayNumber, int collisionMask, bool debug=false)
        {
            float increment = 360f /(float)rayNumber;
            increment *= Mathf.Deg2Rad;
            Vector3 result = desiredDir;
            int hits = 1;

            for (int i = 0; i < rayNumber; i++)
            {
                Vector3 direction = RotateVector2(i * increment, Vector3.right);
                RaycastHit2D hit = Physics2D.Raycast(from, direction, rayLength, ~(1 << 7));
                float length = rayLength;

                if(hit.collider)
                {
                    length = Vector3.Distance(hit.point, from);
                    hits++;
                    result += (from - (Vector3)hit.point)*(1 - length/rayLength);
                }

                #if UNITY_EDITOR
                if(debug)
                {
                    Gizmos.color = hit.collider ? Color.red : Color.white;
                    Gizmos.DrawLine(from, from + direction*length);
                }
                #endif
            }

            return result.normalized;
        }
    }
}