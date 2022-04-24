using System;
using System.Linq;
using System.Reflection;
using UnityEditor.Graphs;
using UnityEngine;

namespace INUlib.BackendToolkit.Audio
{
    /// <summary>
    /// SceneBehaviour is a MonoBehaviour that is the component of a Unity Scene. That is, it is manually
    /// placed in the scene and NOT added dynamically by instantiating a prefab.
    /// All SceneBehaviours must have an Id and they can be fed ServiceLocator services with an attribute,
    /// mimicking a Dependency Injection flow.
    /// </summary>
    public abstract class SceneBehaviour : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private bool _debugInjection;
        [SerializeField] private string _id;
        #endregion

        #region Properties
        public string Id => _id;
        #endregion

        
        #region Methods
        protected virtual void Awake() => LocateServices();

        private void LocateServices()
        {
            Type sbType = this.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var validFields  = sbType.GetFields(flags).Where(x => x.GetCustomAttribute<LocateAttribute>() != null);
            var validProps   = sbType.GetProperties(flags).Where(x => x.GetCustomAttribute<LocateAttribute>() != null);
        
            foreach(FieldInfo field in validFields)
            {
                object service = ServiceLocator.Instance.GetService(field.FieldType);
                if(service == null && _debugInjection)
                    Debug.LogWarning($"There's no service of type {field.FieldType} for property {field.Name}");

                field.SetValue(this, service);
            }

            foreach(PropertyInfo prop in validProps)
            {
                object service = ServiceLocator.Instance.GetService(prop.PropertyType);
                if(service == null && _debugInjection)
                    Debug.LogWarning($"There's no service of type {prop.PropertyType} for property {prop.Name}");

                prop.SetValue(this, service);
            }
        }
        #endregion

    }
}