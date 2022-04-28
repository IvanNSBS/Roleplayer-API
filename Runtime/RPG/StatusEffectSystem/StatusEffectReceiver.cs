using UnityEngine;
using System.Collections.Generic;
using System;

namespace INUlib.RPG.StatusEffectSystem
{
    /// <summary>
    /// Unity Component for GameObjects that can receive StatusEffects
    /// </summary>
    /// <typeparam name="TEffect">The actual StatusEffect type the StatusEffectManager will manage</typeparam>
    /// <typeparam name="TTargets">The actual IStatusEffectTarget the receiver will contain</typeparam>
    public abstract class StatusEffectReceiver<TEffect, TTargets> : MonoBehaviour where TEffect : IStatusEffect where TTargets : IStatusEffectTargets
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
        /// <summary>
        /// Creates the StatusEffectManager on Awake
        /// </summary>
        protected virtual void Awake()
        {
            _manager = new StatusEffectManager<TEffect>();
        }

        /// <summary>
        /// Updates the StatusEffect manager with the time since the last frame
        /// </summary>
        protected virtual void Update()
        {
            _manager.Update(Time.deltaTime);
        }
        #endregion


        #region Methods
        /// <summary>
        /// Calls the ApplyEffect from the StatusEffectManager that the receiver contains
        /// </summary>
        /// <param name="e">The StatusEffect to be applied</param>
        /// <returns>True if effect was reapplied. False otherwise</returns>
        public virtual bool ApplyEffect(TEffect e) => _manager.ApplyEffect(e);

        /// <summary>
        /// Calls the DispelEffect from the StatusEffectManager that the receiver contains
        /// </summary>
        /// <param name="e">The StatusEffect to be dispeled</param>
        public virtual bool DispelEffect(TEffect e) => _manager.DispelEffect(e);

        public void AddOnStatusEffectFinished(Action<TEffect> func) => _manager.onStatusEffectFinished += func;
        public void RemoveOnStatusEffectFinished(Action<TEffect> func) => _manager.onStatusEffectFinished += func;
        #endregion
    }
}