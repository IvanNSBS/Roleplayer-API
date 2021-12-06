using System;
using UnityEngine;

namespace INUlib.RPG.AI.Movement.Steering
{
    interface IMovementPolicy 
    {
        bool HasTarget { get; }
        bool HasReachedTarget { get; }
        void OnUpdate(Vector3 selfPosition);
    }
}