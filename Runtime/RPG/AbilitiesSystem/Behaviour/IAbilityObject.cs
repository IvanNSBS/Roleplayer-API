using System;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// An AbiltiyObject is the game logic for the effect of a certain spell,
    /// such as Adding status effect, instantiating a fireball, etc.
    /// The Ability Object is created as soon as the casting stats, and can gather info about the cast
    /// </summary>
    public interface IAbilityObject<TCaster> where TCaster : IAbilityCaster
    {
        event Action OnFinish;
        void Disable();
        void Activate();
        void Finish();

        void OnUpdate(float deltaTime);
        void OnDrawGizmos();
    }
}