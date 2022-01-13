using System;
using UnityEngine;

namespace INUlib.BackendToolkit.ScriptableValues
{
    [Serializable]
    public class ScriptableReference<TVal, TScriptable> where TScriptable : ScriptableValue<TVal>
    {
        #region Fields
        [SerializeField] private bool m_useConstant;
        [SerializeField] private TVal m_value;
        [SerializeField] private TScriptable m_scriptable;
        #endregion Fields
        
        #region Properties
        public virtual TVal Value
        {
            get => m_useConstant ? m_value : m_scriptable.Value;
        }

        public bool HasScriptable => m_scriptable != null;
        public void AddOnValueChanged(Action<TVal> func) => m_scriptable.AddOnValueChanged(func);
        public void RemoveOnValueChanged(Action<TVal> func) => m_scriptable.RemoveOnValueChanged(func);
        #endregion Properties
    }
}