using System;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// An AbiltiyObject is the game logic for the effect of a certain spell,
    /// such as Adding status effect, instantiating a fireball, etc.
    /// The Ability Object is created as soon as the casting stats, and can gather info about the cast
    /// </summary>
    public interface IAbilityObject
    {
        event Action OnFinishCast;
        event Action OnAbilityFinished;

        void Disable();
        void UnleashAbility();
        void OnUpdate(float deltaTime);
        void OnDrawGizmos();
    }

    public abstract class AbilityObject : IAbilityObject 
    {
        #region Fields
        protected bool _hasUnleashed;
        #endregion

        #region Events
        public event Action OnFinishCast;
        public event Action OnAbilityFinished;
        #endregion


        #region IAbilityObject Methods
        public abstract void Disable();
        public virtual void UnleashAbility() => _hasUnleashed = true;
        public abstract void OnUpdate(float deltaTime);
        public abstract void OnDrawGizmos();
        #endregion


        #region Methods
        public abstract void OnFinishCasting();

        public virtual void FinishCast()
        {
            OnFinishCast?.Invoke();
            OnFinishCasting();
        }

        public virtual void FinishAbility()
        {
            OnAbilityFinished?.Invoke();
            Discard();
        }

        public abstract void Discard();
        #endregion


    }
}