using System;
using UnityEngine;

namespace INUlib.RPG.AI.Movement.Steering.Behaviour
{
    public class FollowPolicy : SteeringPolicy
    {
        #region Constructor
        public FollowPolicy(float acceptDst) : base(acceptDst) { }
        public FollowPolicy(float acceptDst, Transform tgt) : base(acceptDst, tgt) { }
        public FollowPolicy(float acceptDst, Vector3 fixedPos) : base(acceptDst, fixedPos) { }
        #endregion


        #region Methods
        public override void OnUpdate(Vector3 selfPosition)
        {
            if(!HasTarget)
                return;

            CurrentDesiredDirection = CurrentTargetPos - selfPosition;
            HasReachedTarget = Vector3.Distance(CurrentTargetPos, selfPosition) <= _acceptDistance;
        }
        #endregion
    }
}