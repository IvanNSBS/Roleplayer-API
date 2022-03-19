namespace INUlib.RPG.StatusEffectSystem
{
    public interface IStatusEffect
    {
        void Apply();
        void OnApply();
        void OnComplete();
        void OnDispel();
        void OnCollision(StatusEffect ef);
        bool Update(float deltaTime);
    }
}