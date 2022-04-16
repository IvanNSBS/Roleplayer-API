using System;

namespace INUlib.RPG.AbilitiesSystem
{
    public interface IAbilityBase
    {
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

        /// <summary>
        /// Ability category ID. Primarily used for specific Cooldown Reductions
        /// </summary>
        /// <value></value>
        int Category { get; }
    }

    /// <summary>
    /// Interface for a game Ability and the Ability System 
    /// for the INUlib
    /// </summary>
    public interface IAbility<TCaster> : IAbilityBase where TCaster : IAbilityCaster
    {
        /// <summary>
        /// Casts the ability, unleashing it's effect in the world
        /// </summary>
        void Cast(TCaster caster, Action NotifyFinishCast);

        /// <summary>
        /// Function to be called when the Channeling proccess 
        /// of the ability has started
        /// </summary>
        void OnChannelingStarted(TCaster caster);

        /// <summary>
        /// Function to be called when the Channeling proccess 
        /// of the ability has completed and the ability will
        /// then be cast
        /// </summary>
        void OnChannelingCompleted(TCaster caster);


        /// <summary>
        /// Function to be called when the Channeling proccess 
        /// of the ability was canceled
        /// </summary>
        void OnChannelingCanceled(TCaster caster);

        /// <summary>
        /// Getter for the current ability Cooldown
        /// </summary>
        /// <value>Returns the current ability cooldown</value>
        float CurrentCooldown {get; set;}
    }
}