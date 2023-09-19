using UnityEngine;
using System.Collections.Generic;
using System;

namespace INUlib.BackendToolkit
{
    public class ServiceLocator : Singleton<ServiceLocator>
    {
        #region Fields
        private readonly Dictionary<string, object> m_services;
        #endregion Fields
        
        
        #region Constructor
        public ServiceLocator()
        {
            m_services = new Dictionary<string, object>();
        }
        #endregion Constructor
        
        
        #region Methods
        public bool HasService<T>() => m_services.ContainsKey(typeof(T).Name);

        public object GetService(Type t)
        {
            string key = t.Name;
            if (!m_services.ContainsKey(key))
            {
                Debug.LogError($"{key} not registered with {GetType().Name}");
                return null;
            }

            return m_services[key];
        }

        public T GetService<T>() where T : class
        {
            string key = typeof(T).Name;
            if (!m_services.ContainsKey(key))
            {
                Debug.LogError($"{key} not registered with {GetType().Name}");
                return null;
            }

            return (T)m_services[key];
        }
        
        public bool RegisterService<T1>(T1 implementation)
        {
            string key = typeof(T1).Name;
            if (m_services.ContainsKey(key))
            {
                Debug.LogError($"Service <{key}> has been registered already.");
                return false;
            }

            m_services.Add(key, implementation);
            return true;
        }

        public bool RemoveService<T>()
        {
            string key = typeof(T).Name;
            if (!m_services.ContainsKey(key))
            {
                Debug.LogError($"Service of type {key} hasn't been registered.");
                return false;
            }

            m_services.Remove(key);
            return true;
        }
        #endregion Methods
    }
}