using System.Collections.Generic;

namespace INUlib.RPG.StatusEffectSystem
{
    public class StatusEffectManager
    {
        #region Fields
        private List<IStatusEffect> _activeEffects;
        #endregion


        #region Properties
        public IReadOnlyList<IStatusEffect> ActiveEffects => _activeEffects;
        #endregion


        #region Constructor
        public StatusEffectManager()
        {
            _activeEffects = new List<IStatusEffect>();
        }
        #endregion


        #region Methods
        public void ApplyEffect(IStatusEffect effect)
        {
            IStatusEffect sameEffect = null;

            foreach(IStatusEffect e in _activeEffects)
            {
                if(e.GetType() == effect.GetType())
                {
                    sameEffect = e;
                    break;
                }
            }

            if(sameEffect != null) {
                sameEffect.Collide(effect);
            }
            else {
                _activeEffects.Add(effect);
                effect.OnApply();
            }
        }

        public bool DispelEffect(IStatusEffect effect)
        {
            bool dispeled = _activeEffects.Remove(effect);
            if(dispeled)
                effect.OnDispel();

            return dispeled;
        }

        public void Update(float deltaTime)
        {
            for(int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                IStatusEffect effect = _activeEffects[i];
                bool completed = effect.Update(deltaTime);

                if(completed) 
                {
                    effect.OnComplete();
                    _activeEffects.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}