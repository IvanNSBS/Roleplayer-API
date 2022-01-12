using System;
using UnityEngine;

namespace INUlib.Gameplay.AI.Movement
{
    public interface IMovement
    {
        /// <summary>
        /// Event to call when the movement finishes.
        /// Could be a follow, move to, etc.
        /// </summary>
        event Action OnMoveFinished;
        void OnUpdate(Vector3 selfPos, Vector3? targetPos);
        void OnFixedUpdate(Vector3 selfPos, Vector3? targetPos);
    }
}