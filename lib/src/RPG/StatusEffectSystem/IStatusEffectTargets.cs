namespace INUlib.RPG.StatusEffectSystem
{
    /// <summary>
    /// Base interface for the possible targets for a status effect
    /// Things such as a IEntityMover, IDamageable, etc.
    /// A StatusEffectReceiver needs a concrete IStatusEffectTargets to be created
    /// </summary>
    public interface IStatusEffectTargets
    {
    }
}