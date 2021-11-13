using System;
using System.Collections.Generic;
using Essentials.Persistence.Interfaces;

namespace Essentials.Persistence
{
    public static class DataStoreRegistry
    {
        #region Fields
        private static Dictionary<string, Type> m_registry = new Dictionary<string, Type>();
        #endregion Fields

        #region Events
        public static Action<Type> OnStoreRegistered = delegate {  };
        #endregion Events

        
        #region Methods
        public static IReadOnlyDictionary<string, Type> GetRegisteredStores()
        {
            return m_registry;
        }
        
        public static void AddStoreToRegistry<T>() where T : DataStore
        {
            if (m_registry.ContainsKey(TypeToString(typeof(T))))
                return;
            
            m_registry.Add(TypeToString(typeof(T)), typeof(T));
            OnStoreRegistered?.Invoke(typeof(T));
        }

        public static void ClearRegistry()
        {
            m_registry = new Dictionary<string, Type>();
        }
        #endregion Methods
        
        
        #region Helper Methods
        public static string TypeToString(Type type)
        {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }
        #endregion Helper Methods
        
    }
}