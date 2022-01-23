using System;

namespace INUlib.RPG.AbilitiesSystem
{
    /// <summary>
    /// Interface for a game Ability and the Ability System 
    /// for the INUlib
    /// </summary>
    public interface IAbility<TDataHub> where TDataHub : IAbilityDataHub
    {
        /// <summary>
        /// Casts the ability, unleashing it's effect in the world
        /// </summary>
        void Cast(TDataHub dataHub, Action NotifyFinishCast);

        /// <summary>
        /// Function to be called when the Channeling proccess 
        /// of the ability has started
        /// </summary>
        void OnChannelingStarted();

        /// <summary>
        /// Function to be called when the Channeling proccess 
        /// of the ability has completed and the ability will
        /// then be cast
        /// </summary>
        void OnChannelingCompleted();


        /// <summary>
        /// Function to be called when the Channeling proccess 
        /// of the ability was canceled
        /// </summary>
        void OnChannelingCanceled();

        /// <summary>
        /// Getter for the current ability Cooldown
        /// </summary>
        /// <value>Returns the current ability cooldown</value>
        float CurrentCooldown {get; set;}

        /// <summary>
        /// Getter for the ability max cooldown.
        /// </summary>
        /// <value></value>
        float Cooldown {get; set;}

        /// <summary>
        /// Getter for the time that is necessary
        /// to charge the ability to finaly cast it
        /// 0 should mean that the ability is cast instantly
        /// </summary>
        /// <value></value>
        float ChannelingTime {get;}
    }
}