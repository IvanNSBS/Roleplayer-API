using System;
using System.Collections.Generic;

namespace INUlib.RPG.StatusEffectSystem
{
    /// <summary>
    /// Manages all status effects present in an entity
    /// </summary>
    /// <typeparam name="T">The base type for the StatusEffects that will be managed</typeparam>
    public class StatusEffectManager<T> where T : IStatusEffect
    {
        #region Constants
        /// <summary>
        /// Default timer to reset the apply stats for a Status Effect
        /// </summary>
        public const float DEFAULT_EFFECT_STATS_RESET_TIME = 60f;
        #endregion

        #region Fields
        private float _effectStatsResetTime;
        private List<T> _activeEffects;
        private Dictionary<Type, T> _activeEffectsDict;
        private Dictionary<Type, EffectApplyStats> _addedEffectsStats;
        #endregion


        #region Properties
        /// <summary>
        /// List of the current active status effect
        /// </summary>
        public IReadOnlyList<T> ActiveEffects => _activeEffects;

        /// <summary>
        /// EffectApplyStats for each of the status effect types that has been added to this manager
        /// </summary>
        public IReadOnlyDictionary<Type, EffectApplyStats> AddedEffectStats => _addedEffectsStats;
        #endregion


        #region Constructor
        /// <summary>
        /// Creates a StatusEffectManager with the default stats reset time.
        /// </summary>
        public StatusEffectManager()
        {
            _effectStatsResetTime = DEFAULT_EFFECT_STATS_RESET_TIME;
            _activeEffects = new List<T>();
            _activeEffectsDict = new Dictionary<Type, T>();
            _addedEffectsStats = new Dictionary<Type, EffectApplyStats>();
        }

        /// <summary>
        /// Creates a StatusEffectManager with a custom stats reset time.
        /// A -1 to the custom time means that the Apply Stats will never be reset
        /// </summary>
        /// <param name="effectStatsResetTime"></param>
        public StatusEffectManager(float effectStatsResetTime)
        {
            _effectStatsResetTime = effectStatsResetTime;
            _activeEffects = new List<T>();
            _activeEffectsDict = new Dictionary<Type, T>();
            _addedEffectsStats = new Dictionary<Type, EffectApplyStats>();
        }
        #endregion


        #region Methods
        /// <summary>
        /// Returns the ApplyStat for a given status effect or null if the effect has never been
        /// added to the manager
        /// </summary>
        /// <param name="effect">The effect to retrieve the ApplyStats</param>
        /// <returns>The EffectApplyStats for the effect, if it has ever been added. Null otherwise</returns>
        public EffectApplyStats GetEffectApplyStats(T effect)
        {
            if(_addedEffectsStats.ContainsKey(effect.GetType()))
                return _addedEffectsStats[effect.GetType()];
            
            return null;
        }

        /// <summary>
        /// Applies a StatusEffect to the entity.
        /// If there's already a status effect of the same type, the current status effect will be reapplied
        /// and the new one will be passed as te argument for the Reapply function.
        /// Also Creates and/or updates the EffectApplyStats for the StatusEffect
        /// </summary>
        /// <param name="effect">The effect to be added</param>
        public void ApplyEffect(T effect)
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
                sameEffect.Reapply(effect, GetEffectApplyStats(effect));
            }
            else {
                _activeEffects.Add(effect);
                _activeEffectsDict.Add(effect.GetType(), effect);
                effect.Apply(GetEffectApplyStats(effect));
            }

        }

        /// <summary>
        /// Dispells the given StatusEffect, if it is an active effect, prematurely removing it,
        /// canceling it's effect and calling the effect OnDispel function.
        /// Does nothing if the effect is not active in the manager
        /// </summary>
        /// <param name="effect">The effect </param>
        /// <returns>True if the effect was present and thus dispeled. False otherwise</returns>
        public bool DispelEffect(T effect)
        {
            bool dispeled = _activeEffects.Remove(effect);
            if(dispeled)
                effect.OnDispel();

            return dispeled;
        }

        /// <summary>
        /// Updates the status effect manager by a given deltaTime, updating all active status effect
        /// timers and completing and removing them if they have been complete, as well as updating each
        /// status effect ApplyStats
        /// </summary>
        /// <param name="deltaTime">The time since the last frame</param>
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
                    _activeEffectsDict.Remove(effect.GetType());
                }
            }

            foreach(var pair in _addedEffectsStats)
            {
                bool isEffectActive = _activeEffectsDict.ContainsKey(pair.Key);
                EffectApplyStats stats = pair.Value;
                
                stats.Update(deltaTime, isEffectActive);
                if(_effectStatsResetTime > 0 && stats.InactiveTime >= _effectStatsResetTime)
                    stats.Reset();
            }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// Helper function to update or create the EffectApplyStats for a given StatusEffect
        /// </summary>
        /// <param name="ef">The StatusEffect to create or update the EffectApplyStats</param>
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