using System;
using UnityEngine;

namespace INUlib.BackendToolkit.ScriptableValues
{
    public abstract class ScriptableValue<T> : ScriptableObject
    {
        #region Fields
        private T m_value;
        private Action<T> m_onValueChanged;
        #endregion Fields
        
        #region Properties
        public virtual T Value
        {
            get => m_value;
            set
            {
                m_value = value;
                m_onValueChanged?.Invoke(value);
            }
        }
        #endregion Properties
        
        
        #region Methods
        public void AddOnValueChanged(Action<T> function) => m_onValueChanged += function;
        public void RemoveOnValueChanged(Action<T> function) => m_onValueChanged -= function;
        #endregion Methods
    }
}