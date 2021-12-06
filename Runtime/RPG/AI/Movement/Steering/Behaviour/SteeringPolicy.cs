using System;
using UnityEngine;
using INUlib.RPG.AI.Movement.Steering;

namespace INUlib.RPG.AI.Movement.Steering.Behaviour
{
    public abstract class SteeringPolicy : IMovementPolicy
    {
        #region Fields
        protected float _acceptDistance;
        protected Vector3? _fixedPos;
        protected Transform _target;
        #endregion

        #region Properties
        public bool HasTarget => UsingFixedTarget ? _fixedPos.HasValue : _target;
        public Vector3 CurrentMoveDirection { get; protected set; }
        public Vector3 CurrentTargetPos => UsingFixedTarget ? _fixedPos.Value : _target.position;
        public bool UsingFixedTarget { get; set; }
        public bool HasReachedTarget { get; set; }
        #endregion


        #region Constructor
        public SteeringPolicy(float acceptDst)
        {
            _target = null;
            _acceptDistance = acceptDst;
            UsingFixedTarget = false;
        }
        
        public SteeringPolicy(float acceptDst, Transform tgt)
        {
            _target = tgt;
            _acceptDistance = acceptDst;
            UsingFixedTarget = false;
        }

        public SteeringPolicy(float acceptDst, Vector3 fixedPos)
        {
            _acceptDistance = acceptDst;
            _fixedPos = fixedPos;
            UsingFixedTarget = true;
        }
        #endregion


        #region Methods
        public void SetTarget(Transform target)
        {
            _target = target;
            UsingFixedTarget = false;
        }

        public void SetTarget(Vector3 target)
        {
            _fixedPos = target;
            UsingFixedTarget = true;
        }

        public abstract void OnUpdate(Vector3 selfPosition);
        #endregion
    }
}