using System;
using UnityEngine;
using Newtonsoft.Json;

namespace INUlib.Gameplay.AI.Movement.Behaviour
{
    [Serializable]
    public class SteeringData
    {
        #region Inspector Fields
        [SerializeField] [JsonProperty] protected float acceptDistance;
        [SerializeField] [JsonProperty] protected float desiredSpeed;
        [SerializeField] [JsonProperty] protected float maxSteerForce;
        [SerializeField] [JsonProperty] protected MovementType type;
        [SerializeField] [JsonProperty] protected AvoidData avoidData;
        #endregion

        #region Properties
        public float AcceptDistance => acceptDistance;
        public float DesiredSpeed => desiredSpeed;
        public float MaxSteerForce => maxSteerForce;
        public MovementType MoveType => type;
        public AvoidData AvoidData => avoidData;
        #endregion


        public SteeringData(float accept, float desired, float maxForce, AvoidData avoid = null)
        {
            acceptDistance = accept;
            desiredSpeed = desired;
            maxSteerForce = maxForce;
            avoidData = avoid;
        }

        public void SetMovementType(MovementType tp) => type = tp;
        public void SetSpeed(float speed) => desiredSpeed = speed;
        public void SetAvoidData(AvoidData avoid) => avoidData = avoid; 
    }

    [Serializable]
    public class AvoidData
    {
        #region Inspector Fields
        [Range(4, 24)]
        [SerializeField] [JsonProperty] protected int rayAmount;
        [SerializeField] [JsonProperty] protected float rayLength;
        [SerializeField] [JsonProperty] protected int layerMask;
        #endregion

        #region Properties
        public int RayAmount => rayAmount;
        public float RayLength => rayLength;
        public int LayerMask => layerMask;
        #endregion


        public AvoidData(int rayAmnt, float raySize, int layerMask)
        {
            this.rayAmount = rayAmnt;
            this.rayLength = raySize;
            this.layerMask = layerMask;
        }
    }
}