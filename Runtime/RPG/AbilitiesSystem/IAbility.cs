namespace INUlib.RPG.AbilitiesSystem
{
    public interface IAbility
    {
        /// <summary>
        /// Prepares to cast the ability.
        /// This is used to, for example, select and show 
        /// the area that the ability will cover once cast.
        /// You can invoke Cast 
        /// </summary>
        void PrepareCast();
        void Cast();
        float CurrentCooldown {get; set;}
        float Cooldown {get; set;}
        float CastTime {get;}
    }
}