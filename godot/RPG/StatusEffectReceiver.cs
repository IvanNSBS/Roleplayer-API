using Godot;
using System;
using System.Collections.Generic;

namespace INUlib.RPG.StatusEffectSystem
{
    /// <summary>
    /// Unity Component for GameObjects that can receive StatusEffects
    /// </summary>
    /// <typeparam name="TEffect">The actual StatusEffect type the StatusEffectManager will manage</typeparam>
    /// <typeparam name="TTargets">The actual IStatusEffectTarget the receiver will contain</typeparam>
    public abstract partial class StatusEffectReceiver<TEffect, TTargets> : Node where TEffect : IStatusEffect where TTargets : IStatusEffectTargets
    {
        #region Fields
        private StatusEffectController<TEffect> _manager;
        private TTargets _targets;
        #endregion


        #region Properties
        public TTargets Targets => _targets;
        public IReadOnlyList<TEffect> ActiveEffects => _manager.ActiveEffects;
        #endregion


        #region MonoBehaviour Methods
        /// <summary>
        /// Creates the StatusEffectManager on Awake
        /// </summary>
        public override void _Ready()
        {
            _manager = new StatusEffectController<TEffect>();
        }

        /// <summary>
        /// Updates the StatusEffect manager with the time since the last frame
        /// </summary>
        public override void _Process(double deltaTime)
        {
            _manager.Update((float)deltaTime);
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

        /// <summary>
        /// Add a listener to the onStatusEffectFinished event
        /// </summary>
        /// <param name="func">Function that has the StatusEffect that was removed and the index</param>
        public void AddOnStatusEffectFinished(Action<TEffect, int> func) => _manager.onStatusEffectFinished += func;
        
        /// <summary>
        /// Removes a listener from the onStatusEffectFinished event
        /// </summary>
        /// <param name="func">Function that has the StatusEffect that was removed and the index</param>
        public void RemoveOnStatusEffectFinished(Action<TEffect, int> func) => _manager.onStatusEffectFinished -= func;
        
        /// <summary>
        /// Adds a listener to the onStatusEffectAdded event
        /// </summary>
        /// <param name="func">Function that receives the StatusEffect that was applied</param>
        public void AddOnStatusEffectAdded(Action<TEffect> func) => _manager.onStatusEffectAdded += func;
        
        /// <summary>
        /// Removes a listener to the onStatusEffectAdded event
        /// </summary>
        /// <param name="func">Function that receives the StatusEffect that was applied</param>
        public void RemoveOnStatusEffectAdded(Action<TEffect> func) => _manager.onStatusEffectAdded -= func;
        #endregion
    }
}