using UnityEngine;
using Common.Utils.Extensions;

namespace INUlib.RPG.AI.Movement.Behaviour
{
    public class SteeringBehaviour
    {
        #region Fields
        protected float _acceptDistance;
        protected float _desiredSpeed;
        protected float _maxSteerForce;
        protected Transform _target;
        protected MovementType _type;
        #endregion

        #region Properties
        public virtual bool HasTarget => _target;
        public virtual Vector3 DesiredSpeed { get; protected set; }
        public virtual Vector3 TargetPos => _target.position;
        public virtual bool HasReachedTarget { get; set; }
        #endregion


        #region Constructor
        public SteeringBehaviour(float acceptDst, float desiredSpeed, float maxForce)
        {
            _maxSteerForce = maxForce;
            _acceptDistance = acceptDst;
            _desiredSpeed = desiredSpeed;
            _target = null;
        }
        
        public SteeringBehaviour(float acceptDst, float desiredSpeed, Transform tgt)
        {
            _target = tgt;
            _acceptDistance = acceptDst;
        }
        #endregion


        #region Methods
        public void SetTarget(Transform target) => _target = target;

        public void SetMovementType(MovementType type) => _type = type;

        public virtual void CalculateDesiredSpeed(Vector3 selfPosition)
        {
            switch (_type)
            {
                case MovementType.Follow:
                {
                    Follow(selfPosition);
                    //Decorate Follow here
                    // DesiredSpeed = SteerDirection.Avoid(selfPosition, _desired, _colliderSize*2f, _rays, 0, false);

                    break;
                }
                case MovementType.Flee:
                {
                    Flee(selfPosition);
                    break;
                }
                default:
                {
                    DesiredSpeed = Vector3.zero;
                    break;
                }
            }
        }
        #endregion


        #region Behaviour Methods
        private void Follow(Vector3 selfPosition)
        {
            if(!HasTarget)
            {
                DesiredSpeed = Vector3.zero;
                HasReachedTarget = false;
                return;
            }

            float factor = ArriveFactor(selfPosition, TargetPos, _acceptDistance, _maxSteerForce);
            DesiredSpeed = (TargetPos - selfPosition).normalized * factor * _desiredSpeed;
            HasReachedTarget = Vector3.Distance(TargetPos, selfPosition) <= _acceptDistance;
        }

        private void Flee(Vector3 selfPosition)
        {
            if(!HasTarget)
            {
                DesiredSpeed = Vector3.zero;
                HasReachedTarget = false;
                return;
            }

            float factor = ArriveFactor(selfPosition, TargetPos, _acceptDistance, _maxSteerForce);
            DesiredSpeed = (selfPosition - TargetPos).normalized * factor * _desiredSpeed;
            HasReachedTarget = Vector3.Distance(TargetPos, selfPosition) >= _acceptDistance;
        }
        #endregion


        #region Utility Methods
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
                Vector3 direction = Vector2.right.RotateDegrees(i*increment);
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
        #endregion
    }
}