using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace INUlib.RPG.StatusEffectSystem
{
    public class StatusEffectManager
    {
        #region Fields
        private List<StatusEffect> _activeEffects;
        #endregion


        #region Properties
        public IReadOnlyList<StatusEffect> ActiveEffects => _activeEffects;
        #endregion


        #region Constructor
        public StatusEffectManager()
        {
            _activeEffects = new List<StatusEffect>();
        }
        #endregion


        #region Methods
        public void ApplyEffect(StatusEffect effect)
        {
            StatusEffect sameEffect = null;

            foreach(StatusEffect e in _activeEffects)
            {
                if(e.GetType() == effect.GetType())
                {
                    sameEffect = e;
                    break;
                }
            }

            if(sameEffect != null) {
                sameEffect.OnCollision(effect);
            }
            else {
                _activeEffects.Add(effect);
                effect.OnApply();
            }
        }

        public bool DispelEffect(StatusEffect effect)
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
                StatusEffect effect = _activeEffects[i];
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