using System;

namespace INUlib.RPG.AbilitiesSystem
{
    public interface IAbilityBase
    {
        /// <summary>
        /// The cast 
        /// </summary>
        AbilityCastType AbilityCastType { get; }
        
        /// <summary>
        /// Getter for the ability max cooldown.
        /// </summary>
        /// <value></value>
        float Cooldown { get; set; }
        
        /// <summary>
        /// Getter for the TimelineData used for a spell
        /// </summary>
        /// <returns></returns>
        TimelineData CastTimeline { get; }

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
    public interface IAbility<TCaster> : IAbilityBase where TCaster : ICasterInfo
    {
        /// <summary>
        /// Casts the ability, unleashing it's effect in the world
        /// </summary>
        CastObjects Cast(TCaster caster);

        /// <summary>
        /// Whether or not a caster can cast an ability
        /// </summary>
        /// <param name="caster"></param>
        /// <returns></returns>
        bool CanCast(TCaster caster);
    }

    public class CastObjects
    {
        public readonly CastHandlerPolicy policy;
        public readonly AbilityObject abilityObject;
        public readonly CastTimeline timeline;
        
        public CastObjects(CastHandlerPolicy policy, AbilityObject abilityObject, TimelineData timelineData)
        {
            this.timeline = new CastTimeline(timelineData);
            this.policy = policy;
            this.abilityObject = abilityObject;
        }
    } 
}