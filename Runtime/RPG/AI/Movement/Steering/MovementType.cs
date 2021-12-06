using System;

namespace INUlib.RPG.AI.Movement.Steering.Components
{
    [Serializable]
    public enum MovementType
    {
        Wander = 0,
        Follow = 1,
        Flee = 2
    }
}