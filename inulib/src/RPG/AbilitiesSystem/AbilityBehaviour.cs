using System;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// An AbiltiyObject is the game logic for the effect of a certain spell,
    /// such as Adding status effect, instantiating a fireball, etc.
    /// The Ability Object is created as soon as the casting stats, and can gather info about the entire cast process.
    /// but it must only start affecting the world after the channeling starts through the UnleashAbility method
    /// </summary>
    public abstract class AbilityBehaviour
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
        /// <summary>
        /// Event to be called when the ability has been completed and it can be
        /// discarded by the garbage collector
        /// </summary>
        public event Action NotifyDiscard;
        #endregion
        

        #region Methods
        /// <summary>
        /// Disables the effect of the spell when, and only when, it has just been
        /// instantiated by the IAbility. 
        /// Necessary primarily for IAbilityObjects that are MonoBehaviours and need to start disabled
        /// before being Unleashed
        /// </summary>
        public virtual void Disable() { }

        /// <summary>
        /// Function that contains the logic to make the ability be released in the game world
        /// </summary>
        public virtual void OnAbilityUnleashed()
        {
            _hasUnleashed = true;
        }
        
        public virtual void OnNewCastRequested(int currentCastIndex, CastingState currentCastState) { }

        /// <summary>
        /// What to do on a frame update
        /// </summary>
        /// <param name="deltaTime">How much time has passed since the last frame</param>
        public virtual void OnUpdate(float deltaTime, CastingState state)
        {
            _elapsedTime += deltaTime;
        }

        /// <summary>
        /// What to do when Overchanneling Starts and during it's progress
        /// </summary>
        /// <param name="currentTime">The current time in the overchanneling process</param>
        /// <param name="overChannelDuration">How much time the overchannelling process will last</param>
        public virtual void OnOverchannel(float currentTime, float overChannelDuration) { }

        /// <summary>
        /// How to draw Unity Gizmos
        /// </summary>
        public virtual void OnDrawGizmos() { }

        /// <summary>
        /// Ends the ability life, calling the Discard logic and removing it from the AbilitiesController
        /// Active Ability Objects, marking it to be collectd by the GC.
        /// </summary>
        public virtual void InvokeNotifyDiscard()
        {
            NotifyDiscard?.Invoke();
        }
        
        /// <summary>
        /// Defines what should happen to the ability object when the cast has been forcefully interrupted
        /// by an external factor such as a stun status effect.
        /// </summary>
        public virtual void OnForcedInterrupt() { }
        
        /// <summary>
        /// Defines what should happen to the ability object when the user inputs a cancel cast.
        /// </summary>
        public virtual void OnCancelRequested(CastingState currentCastState) { }
        
        public virtual void OnCastFinishedConcentrationStartedAndConcentrationStarted() { }
        public virtual void OnChannelingFinishedAndOverchannelingStarted() { }
        public virtual void OnOverChannelingFinishedAndCastStarted() { }
        public virtual void OnRecoveryFinished() { }
        
        public virtual void OnConcentrationFinishedAndRecoveryStarted() { }
        #endregion
    }
}