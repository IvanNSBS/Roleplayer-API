namespace INUlib.RPG.StatusEffectSystem
{
    public interface IStatusEffect
    {
        void Apply(EffectApplyStats stats);
        void OnComplete();
        void OnDispel();
        void Reapply(IStatusEffect ef, EffectApplyStats stats);
        bool Update(float deltaTime);
    }
}