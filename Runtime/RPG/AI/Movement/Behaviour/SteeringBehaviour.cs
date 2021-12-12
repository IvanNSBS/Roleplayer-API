using System;
using UnityEngine;
using INUlib.RPG.AI.Movement;
using INUlib.RPG.AI.Movement.Components;

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
                    if(!HasTarget)
                    {
                        DesiredSpeed = Vector3.zero;
                        HasReachedTarget = false;
                        break;
                    }

                    float factor = SteerDirection.ArriveFactor(selfPosition, TargetPos, _acceptDistance, _maxSteerForce);
                    DesiredSpeed = (TargetPos - selfPosition).normalized * factor * _desiredSpeed;
                    HasReachedTarget = Vector3.Distance(TargetPos, selfPosition) <= _acceptDistance;
                    break;
                }
                case MovementType.Flee:
                {
                    if(!HasTarget)
                        return;

                    DesiredSpeed = (selfPosition - TargetPos).normalized * _desiredSpeed;
                    HasReachedTarget = Vector3.Distance(TargetPos, selfPosition) >= _acceptDistance;
                    break;
                }
                default:
                {
                    DesiredSpeed = Vector3.right;
                    break;
                }
            }
        }
        #endregion
    }
}