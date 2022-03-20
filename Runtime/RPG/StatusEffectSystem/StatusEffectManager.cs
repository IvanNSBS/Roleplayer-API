using System;
using System.Collections.Generic;

namespace INUlib.RPG.StatusEffectSystem
{
    public class StatusEffectManager
    {
        #region Constants
        public const float DEFAULT_EFFECT_STATS_RESET_TIME = 60f;
        #endregion

        #region Fields
        private float _effectStatsResetTime;
        private List<IStatusEffect> _activeEffects;
        private Dictionary<Type, IStatusEffect> _activeEffectsDict;
        private Dictionary<Type, EffectApplyStats> _addedEffectsStats;
        #endregion


        #region Properties
        public IReadOnlyList<IStatusEffect> ActiveEffects => _activeEffects;
        public IReadOnlyDictionary<Type, EffectApplyStats> AddedEffectStats => _addedEffectsStats;
        #endregion


        #region Constructor
        public StatusEffectManager()
        {
            _effectStatsResetTime = DEFAULT_EFFECT_STATS_RESET_TIME;
            _activeEffects = new List<IStatusEffect>();
            _activeEffectsDict = new Dictionary<Type, IStatusEffect>();
            _addedEffectsStats = new Dictionary<Type, EffectApplyStats>();
        }

        public StatusEffectManager(float effectStatsResetTime)
        {
            _effectStatsResetTime = effectStatsResetTime;
            _activeEffects = new List<IStatusEffect>();
            _activeEffectsDict = new Dictionary<Type, IStatusEffect>();
            _addedEffectsStats = new Dictionary<Type, EffectApplyStats>();
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

            AddOrUpdateEffectStats(effect);

            if(sameEffect != null) {
                sameEffect.Reapply(effect, _addedEffectsStats[effect.GetType()]);
            }
            else {
                _activeEffects.Add(effect);
                _activeEffectsDict.Add(effect.GetType(), effect);
                effect.Apply(_addedEffectsStats[effect.GetType()]);
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
            foreach(var pair in _addedEffectsStats)
            {
                bool isEffectActive = _activeEffectsDict.ContainsKey(pair.Key);
                EffectApplyStats stats = pair.Value;
                
                stats.Update(deltaTime, isEffectActive);
                if(_effectStatsResetTime > 0 && stats.InactiveTime >= _effectStatsResetTime)
                    stats.Reset();
            }

            for(int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                IStatusEffect effect = _activeEffects[i];
                bool completed = effect.Update(deltaTime);

                if(completed) 
                {
                    effect.OnComplete();
                    _activeEffects.RemoveAt(i);
                    _activeEffectsDict.Remove(effect.GetType());
                }
            }
        }
        #endregion


        #region Helper Methods
        private void AddOrUpdateEffectStats(IStatusEffect ef)
        {
            Type efType = ef.GetType();

            /// If the effect has never been added, create the stat for it
            if(!_addedEffectsStats.ContainsKey(efType))
            {
                EffectApplyStats stats = new EffectApplyStats();
                stats.TimesApplied = 1;
                _addedEffectsStats.Add(efType, stats);
            }
            else
            {
                EffectApplyStats stats = _addedEffectsStats[efType];
                stats.TimesApplied++;
                stats.InactiveTime = 0f;
                stats.SecondsSinceLastApply = 0f;
            }
        }
        #endregion
    }
}