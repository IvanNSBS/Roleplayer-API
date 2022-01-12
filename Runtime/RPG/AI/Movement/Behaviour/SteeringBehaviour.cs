using UnityEngine;
using INUlib.Utils.Extensions;
using System;

namespace INUlib.RPG.AI.Movement.Behaviour
{
    public class SteeringBehaviour : IMovement
    {
        #region Fields
        public event Action OnMoveFinished;
        protected SteeringData _steeringData;
        protected Rigidbody2D _rb;
        #endregion

        #region Properties
        public virtual SteeringData SteeringData => _steeringData;
        public virtual Vector3 DesiredSpeed { get; protected set; }
        public virtual bool DebugAvoid {get; set;}
        #endregion


        #region Constructor
        public SteeringBehaviour(Rigidbody2D rb, SteeringData data)
        {
            _rb = rb;
            _steeringData = data;
        }

        public SteeringBehaviour(Rigidbody2D rb, float acceptDst, float desiredSpeed, float maxForce, AvoidData avoid = null)
        {
            _rb = rb;
            _steeringData = new SteeringData(acceptDst, desiredSpeed, maxForce, avoid);
        }
        #endregion


        #region Methods
        public void SetMovementType(MovementType type) => SteeringData.SetMovementType(type);
        public void SetSteeringData(SteeringData data) => _steeringData = data; 

        public void OnUpdate(Vector3 selfPos, Vector3? targetPos)
        {
            if(!targetPos.HasValue)
            {
                DesiredSpeed = Vector3.zero;
                return;
            }

            CalculateDesiredSpeed(selfPos, targetPos.Value);
        }

        public void OnFixedUpdate(Vector3 selfPos, Vector3? targetPos)
        {
            if(!targetPos.HasValue)
                return;

            ApplyForces();
        }

        public virtual void CalculateDesiredSpeed(Vector3 selfPosition, Vector3 targetPos)
        {
            switch (SteeringData.MoveType)
            {
                case MovementType.Follow:
                {
                    Follow(selfPosition, targetPos);
                    if(SteeringData.AvoidData != null)
                        DesiredSpeed = Avoid(selfPosition, DesiredSpeed, 
                            SteeringData.AvoidData.RayLength, SteeringData.AvoidData.RayAmount, 
                            SteeringData.AvoidData.LayerMask, DebugAvoid);

                    break;
                }
                case MovementType.Flee:
                {
                    Flee(selfPosition, targetPos);
                    break;
                }
                default:
                {
                    DesiredSpeed = Vector3.zero;
                    break;
                }
            }
        }

        protected void ApplyForces()
        {
            var steer = (Vector2)DesiredSpeed - _rb.velocity;
            if(steer.magnitude > _steeringData.MaxSteerForce)
                steer = steer.normalized * _steeringData.MaxSteerForce;

            _rb.AddForce(steer);

            float finalDesired = DesiredSpeed.magnitude*Time.fixedDeltaTime;
            if(_rb.velocity.magnitude > finalDesired)
                _rb.velocity = _rb.velocity.normalized*finalDesired;
        }
        #endregion


        #region Behaviour Methods
        private void Follow(Vector3 selfPosition, Vector3 targetPos)
        {
            float factor = ArriveFactor(selfPosition, targetPos, SteeringData.AcceptDistance, SteeringData.MaxSteerForce);
            DesiredSpeed = (targetPos - selfPosition).normalized * factor * SteeringData.DesiredSpeed;
            
            bool hasReachedTarget = factor <= 0.005f;
            if(hasReachedTarget)
                OnMoveFinished?.Invoke();
        }

        private void Flee(Vector3 selfPosition, Vector3 targetPos)
        {
            float factor = ArriveFactor(selfPosition, targetPos, SteeringData.AcceptDistance, SteeringData.MaxSteerForce);
            DesiredSpeed = (selfPosition - targetPos).normalized * factor * SteeringData.DesiredSpeed;
            
            bool hasReachedTarget = factor <= 0.005f;
            if(hasReachedTarget)
                OnMoveFinished?.Invoke();
        }
        #endregion


        #region Utility Methods
        public static Vector3 Avoid(Vector3 from, Vector3 desiredVel, float radius, int rayNumber, int collisionMask, bool debug=false)
        {
            float increment = 360f /(float)rayNumber;
            increment *= Mathf.Deg2Rad;

            float desiredLenght = desiredVel.magnitude;
            Vector2 dir = desiredVel.normalized;

            Vector3 result = desiredVel;
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
                }

                #if UNITY_EDITOR
                if(debug)
                {
                    var color = hit.collider ? Color.red : Color.white;
                    Debug.DrawLine(from, from + direction * length, color, Time.deltaTime);
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