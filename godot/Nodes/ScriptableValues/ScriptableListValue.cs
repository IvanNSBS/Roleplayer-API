using System;
using UnityEngine;
using System.Collections.Generic;

namespace INUlib.BackendToolkit.ScriptableValues
{
    public abstract class ScriptableListValue<T> : ScriptableValue<List<T>>
    {
        #region Inspector Fields
        [SerializeField] private bool _resetOnEnable;
        #endregion


        #region Fields
        public event Action<T> onElementAdded;
        public event Action<T, int> onElementRemoved;
        #endregion Fields
                
        
        #region Methods
        private void OnEnable()
        {
            if(_resetOnEnable)
                Value = new List<T>();
        }

        public void AddElement(T el) 
        {
            Value.Add(el);
            onElementAdded?.Invoke(el);
        }

        public void RemoveElement(T el)
        {
            if(Value == null)
                return;
            
            int idx = Value.FindIndex(x => x.Equals(el));
            if(idx != -1)
            {
                Value.RemoveAt(idx);
                onElementRemoved?.Invoke(el, idx);
            }
        }
        #endregion Methods
    }
}