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
        CastObjects Cast(TCaster caster);
    }

    public class CastObjects
    {
        public readonly CastHandlerPolicy policy;
        public readonly IAbilityObject abilityObject;
        
        public CastObjects(CastHandlerPolicy policy, IAbilityObject abilityObject)
        {
            this.policy = policy;
            this.abilityObject = abilityObject;
        }
    } 
}