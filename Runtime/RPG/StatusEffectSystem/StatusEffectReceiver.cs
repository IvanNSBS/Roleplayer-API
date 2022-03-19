using UnityEngine;
using System.Collections.Generic;

namespace INUlib.RPG.StatusEffectSystem
{
    public abstract class StatusEffectReceiver<T, TTargets> 
    : MonoBehaviour 
    where TTargets : IStatusEffectTargets
    {
        #region Fields
        private StatusEffectManager _manager;
        [SerializeField] private TTargets _targets;
        #endregion


        #region Properties
        public TTargets Targets => _targets;
        public IReadOnlyList<IStatusEffect> ActiveEffects => _manager.ActiveEffects;
        #endregion


        #region MonoBehaviour Methods
        protected virtual void Awake()
        {
            _manager = new StatusEffectManager();
        }

        protected virtual void Update()
        {
            _manager.Update(Time.deltaTime);
        }
        #endregion


        #region Methods
        public void ApplyEffect(IStatusEffect e) => _manager.ApplyEffect(e);
        public bool DispelEffect(IStatusEffect e) => _manager.DispelEffect(e);
        #endregion
    }
}