using UnityEngine;
using System.Collections.Generic;

namespace INUlib.RPG.StatusEffectSystem
{
    public abstract class StatusEffectReceiver<TEffect, TTargets> 
    : MonoBehaviour 
    where TEffect : IStatusEffect
    where TTargets : IStatusEffectTargets
    {
        #region Fields
        private StatusEffectManager<TEffect> _manager;
        [SerializeField] private TTargets _targets;
        #endregion


        #region Properties
        public TTargets Targets => _targets;
        public IReadOnlyList<TEffect> ActiveEffects => _manager.ActiveEffects;
        #endregion


        #region MonoBehaviour Methods
        protected virtual void Awake()
        {
            _manager = new StatusEffectManager<TEffect>();
        }

        protected virtual void Update()
        {
            _manager.Update(Time.deltaTime);
        }
        #endregion


        #region Methods
        public void ApplyEffect(TEffect e) => _manager.ApplyEffect(e);
        public bool DispelEffect(TEffect e) => _manager.DispelEffect(e);
        #endregion
    }
}