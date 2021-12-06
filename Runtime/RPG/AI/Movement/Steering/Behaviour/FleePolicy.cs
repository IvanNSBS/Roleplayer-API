using System;
using UnityEngine;

namespace INUlib.RPG.AI.Movement.Steering.Behaviour
{
    public class FleePolicy : SteeringPolicy
    {
        #region Constructor
        public FleePolicy(float acceptDst) : base(acceptDst) { }
        public FleePolicy(float acceptDst, Transform tgt) : base(acceptDst, tgt) { }
        public FleePolicy(float acceptDst, Vector3 fixedPos) : base(acceptDst, fixedPos) { }
        #endregion


        #region Methods
        public override void OnUpdate(Vector3 selfPosition)
        {
            if(!HasTarget)
                return;

            CurrentMoveDirection = (selfPosition - CurrentTargetPos).normalized;
            CurrentMoveDirection = SteerDirection.AverageVector(selfPosition, CurrentMoveDirection, _acceptDistance, 24, 0);

            HasReachedTarget = Vector3.Distance(CurrentTargetPos, selfPosition) >= _acceptDistance;
        }
        #endregion
    }
}