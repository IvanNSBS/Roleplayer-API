using System;
using UnityEngine;

namespace INUlib.RPG.AI.Movement.Steering.Behaviour
{
    public class FollowPolicy
    {
        #region Fields
        private float _acceptDistance;
        private Vector3 _fixedPos;
        private Transform _target;
        #endregion

        #region Properties
        public Vector3 CurrentMoveDirection { get; private set; }
        public Vector3 CurrentTargetPos => FixedTarget ? _fixedPos : _target.position;
        public bool FixedTarget { get; set; }
        public bool HasReachedTarget { get; set; }
        #endregion


        #region Constructor
        public FollowPolicy(Transform target, float acceptDst)
        {
            _target = target;
            FixedTarget = false;
            _acceptDistance = acceptDst;
        }

        public FollowPolicy(Vector3 pos, float acceptDst)
        {
            _fixedPos = pos;
            FixedTarget = true;
            _acceptDistance = acceptDst;
        }

        public FollowPolicy(Vector3 fixedPos, Transform tgt, float acceptDst, bool fixedTgt=false)
        {
            _acceptDistance = acceptDst;
            _fixedPos = fixedPos;
            _target = tgt;
            FixedTarget = fixedTgt;
        }
        #endregion


        #region Methods
        public void OnUpdate(Vector3 selfPosition)
        {
            CurrentMoveDirection = (CurrentTargetPos - selfPosition).normalized;
            CurrentMoveDirection = SteerDirection.AverageVector(selfPosition, CurrentMoveDirection, _acceptDistance, 24, 0);

            HasReachedTarget = Vector3.Distance(CurrentTargetPos, selfPosition) <= _acceptDistance;
        }
        #endregion
    }
}