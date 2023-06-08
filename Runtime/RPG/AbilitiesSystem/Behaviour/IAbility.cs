using System;

namespace INUlib.RPG.AbilitiesSystem
{
    public interface IAbilityBase
    {
        /// <summary>
        /// The cast type of the spell 
        /// </summary>
        AbilityCastType AbilityCastType { get; }

        /// <summary>
        /// Whether to start cooldown after casting or after channeling
        /// </summary>
        StartCooldownPolicy StartCooldownPolicy { get; }
        
        /// <summary>
        /// Whether to manually discard the AbilityObject from withing or
        /// automatically after cast timeline finishes
        /// </summary>
        DiscardPolicy DiscardPolicy { get; }
        
        /// <summary>
        /// Getter for the ability max cooldown.
        /// </summary>
        /// <value></value>
        float Cooldown { get; set; }
        
        /// <summary>
        /// How many times an ability can be used.
        /// A charge will be recovered once the cooldown time has been completed,
        /// until the max charges are reached.
        /// Ability with charges greater than one can be cast even while on cooldown given that there are enough
        /// charges to be consumed.
        /// An ability will consume charges once the ability has the effect unleashed in the world
        /// by the AbilityBehaviour API. This means that unlike charge recovery, charges consumption is not
        /// directly related to cooldown, although a StartCooldownPolicy of AfterUnleash can be used to
        /// link them.
        /// </summary>
        int Charges { get; }
        
        /// <summary>
        /// Getter for the TimelineData used for a spell
        /// </summary>
        /// <returns></returns>
        TimelineData CastTimeline { get; }

        /// <summary>
        /// Getter for an array of TimelineData.
        /// It will be used for subsequent cast requests on a cast handler.
        /// </summary>
        TimelineData[] OnRecastTimelines { get; }
        
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
        /// <param name="caster"></param>
        /// <param name="fromSlot">The slot this ability was cast from</param>
        /// <returns></returns>
        CastObjects Cast(TCaster caster, uint fromSlot);

        /// <summary>
        /// Whether or not a caster can cast an ability
        /// </summary>
        /// <param name="caster"></param>
        /// <returns></returns>
        bool CanCast(TCaster caster);
    }

    /// <summary>
    /// Container for all information needed to be passed to the CastHandler once the ability is cast
    /// </summary>
    public class CastObjects
    {
        public readonly AbilityBehaviour AbilityBehaviour;
        public CastTimeline timeline;
        public readonly Func<bool> endConcentrationCondition;
        
        /// <summary>
        /// Constructor for this info container
        /// </summary>
        /// <param name="abilityBehaviour">The ability behaviour instantiated by the Ability factory</param>
        /// <param name="timelineData">The Timeline created by the Ability factory</param>
        /// <param name="endConcentrationCondition">
        /// The function that determines when the ability behaviour can finish the concentration.
        /// Has no effect if ability is not of concentration type and can be set to null in those cases.
        /// If ability is of concentration type and this callback is set to null it means that the
        /// ability will stay on Concentration state indefinitely and will only leave this state
        /// from explicitly calling FinishConcentration from the AbilitiesController.
        /// </param>
        public CastObjects(AbilityBehaviour abilityBehaviour, TimelineData timelineData, Func<bool> endConcentrationCondition)
        {
            this.timeline = new CastTimeline(timelineData);
            this.AbilityBehaviour = abilityBehaviour;
            this.endConcentrationCondition = endConcentrationCondition;
        }
    } 
}