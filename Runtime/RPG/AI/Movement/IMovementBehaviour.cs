using System;
using UnityEngine;

namespace INUlib.RPG.AI.Movement
{
    interface IMovementBehaviour 
    {
        bool HasTarget { get; }
        bool HasReachedTarget { get; }
        void OnUpdate(Vector3 selfPosition);
    }
}