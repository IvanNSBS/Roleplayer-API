namespace INUlib.RPG.AbilitiesSystem
{
    public interface IAbility
    {
        void Cast();
        float CurrentCooldown {get; set;}
        float Cooldown {get; set;}
        float CastTime {get;}
    }
}