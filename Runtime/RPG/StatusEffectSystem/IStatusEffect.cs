namespace INUlib.RPG.StatusEffectSystem
{
    public interface IStatusEffect
    {
        void Apply();
        void OnApply();
        void OnComplete();
        void OnDispel();
        void Collide(IStatusEffect ef);
        bool Update(float deltaTime);
    }
}