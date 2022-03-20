namespace INUlib.RPG.StatusEffectSystem
{
    public interface IStatusEffect
    {
        void Apply();
        void OnComplete();
        void OnDispel();
        void Reapply(IStatusEffect ef);
        bool Update(float deltaTime);
    }
}