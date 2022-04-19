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
        /// <summary>
        /// Event to be fired when the ability cast process was finished
        /// and the CastState will be marked as None
        /// </summary>
        event Action OnFinishCast;

        /// <summary>
        /// Event to be called when the ability has been completed and it can be
        /// discarded by the garbage collector
        /// </summary>
        event Action OnAbilityFinished;

        /// <summary>
        /// Disables the effect of the spell when, and only when, it has just been
        /// instantiated by the IAbility. 
        /// Necessary primarily for IAbilityObjects that are MonoBehaviours and need to start disabled
        /// before being Unleashed
        /// </summary>
        void Disable();

        /// <summary>
        /// Function that contains the logic to make the ability be released in the game world
        /// </summary>
        void UnleashAbility();

        /// <summary>
        /// What to do on a frame update
        /// </summary>
        /// <param name="deltaTime">How much time has passed since the last frame</param>
        void OnUpdate(float deltaTime);

        /// <summary>
        /// How to draw Unity Gizmos
        /// </summary>
        void OnDrawGizmos();
    }

    public abstract class AbilityObject : IAbilityObject 
    {
        #region Fields
        /// <summary>
        /// Boolean that checks if the ability has been already unleashed or not
        /// </summary>
        protected bool _hasUnleashed;

        /// <summary>
        /// How many time has passed so far for the entire cast process
        /// </summary>
        protected float _elapsedTime;
        #endregion

        #region Events
        public event Action OnFinishCast;
        public event Action OnAbilityFinished;
        #endregion


        #region IAbilityObject Methods
        public abstract void Disable();
        public virtual void UnleashAbility() => _hasUnleashed = true;
        public virtual void OnUpdate(float deltaTime) => _elapsedTime += deltaTime;
        public abstract void OnDrawGizmos();
        #endregion


        #region Methods
        /// <summary>
        /// Defines the behaviour for when the ability
        /// </summary>
        public abstract void OnFinishCasting();

        /// <summary>
        /// Finishes the Ability Cast, invoking the OnFinishCast event and calling the OnFinishCasting 
        /// behaviour, marking the actor cast state as None.
        /// </summary>
        public void FinishCast()
        {
            OnFinishCast?.Invoke();
            OnFinishCasting();
        }

        /// <summary>
        /// Ends the ability life, calling the Discard logic and removing it from the AbilitiesController
        /// Active Ability Objects, marking it to be collectd by the GC.
        /// </summary>
        public void DiscardAbility()
        {
            OnAbilityFinished?.Invoke();
            Discard();
        }

        /// <summary>
        /// Defines how an ability will be discarded once the effect has finished.
        /// This functions should define how the garbabe created by the spell will be collected.
        /// Can be empty if nothing should be done.
        /// </summary>
        public abstract void Discard();
        #endregion
    }
}