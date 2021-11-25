using System;
using UnityEngine;

namespace Core.ScriptableValues
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
        #endregion Properties
    }
}