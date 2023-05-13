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
        /// Getter for the time that is necessary
        /// to charge the ability to finaly cast it
        /// 0 should mean that the ability is cast instantly
        /// </summary>
        /// <value></value>
        float ChannelingTime { get; }

        /// <summary>
        /// Getter for the time that is necessary to finish performing
        /// the ability after it has been successfully channeled.
        /// Can be perceived like a cooldown needed to be able to 
        /// perform other actions again
        /// </summary>
        float RecoveryTime { get; }

        // CastTimeline CastTimeline();

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
        
        public CastObjects(CastHandlerPolicy policy, AbilityObject abilityObject)
        {
            this.policy = policy;
            this.abilityObject = abilityObject;
        }
    } 
}