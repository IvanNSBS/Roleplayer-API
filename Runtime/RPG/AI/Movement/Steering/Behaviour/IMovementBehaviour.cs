using System;
using UnityEngine;

namespace INUlib.RPG.AI.Movement.Steering
{
    interface IMovementBehaviour 
    {
        bool IsMoving { get; }
        event Action OnTargetReached;
        event Action OnMovementStopped;
        void StopMovement();
        void StartMovement();
        void OnUpdate();
        void OnFixedUpdate();
    }
}