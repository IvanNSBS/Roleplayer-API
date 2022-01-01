using System;
using UnityEngine;

namespace INUlib.RPG.AI.Movement.Behaviour
{
    [Serializable]
    public class SteeringData
    {
        #region Inspector Fields
        [SerializeField] protected float _acceptDistance;
        [SerializeField] protected float _desiredSpeed;
        [SerializeField] protected float _maxSteerForce;
        [SerializeField] protected MovementType _type;
        [SerializeField] protected AvoidData _avoidData;
        #endregion

        #region Properties
        public float AcceptDistance => _acceptDistance;
        public float DesiredSpeed => _desiredSpeed;
        public float MaxSteerForce => _maxSteerForce;
        public MovementType MoveType => _type;
        public AvoidData AvoidData => _avoidData;
        #endregion


        public SteeringData(float accept, float desired, float maxForce, AvoidData avoid = null)
        {
            _acceptDistance = accept;
            _desiredSpeed = desired;
            _maxSteerForce = maxForce;
            _avoidData = avoid;
        }

        public void SetMovementType(MovementType tp) => _type = tp;
        public void SetAvoidData(AvoidData avoid) => _avoidData = avoid; 
    }

    [Serializable]
    public class AvoidData
    {
        #region Inspector Fields
        [Range(4, 24)]
        [SerializeField] protected int _rayAmount;
        [SerializeField] protected float _rayLength;
        [SerializeField] protected int _layerMask;
        #endregion

        #region Properties
        public int RayAmount => _rayAmount;
        public float RayLength => _rayLength;
        public int LayerMask => _layerMask;
        #endregion


        public AvoidData(int rayAmnt, float raySize, int layerMask)
        {
            this._rayAmount = rayAmnt;
            this._rayLength = raySize;
            this._layerMask = layerMask;
        }
    }
}