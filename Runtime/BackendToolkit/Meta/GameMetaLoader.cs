using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace INUlib.BackendToolkit.Meta
{
    /// <summary>
    /// Helper Meta Loader Class that will automatically instantiate and load
    /// a IMetaFile through reflection for all properties/fields that are marked with
    /// the MetaAttribute on the default constructor 
    /// </summary>
    public abstract class GameMetaLoader
    {
        #region Constructors
        public GameMetaLoader() => LoadAllMarkedMetas();
        public GameMetaLoader(bool debug) => LoadAllMarkedMetas(debug);
        #endregion


        #region Methods
        /// <summary>
        /// Loads all fields and properties marked with the Meta Attribute
        /// </summary>
        protected void LoadAllMarkedMetas(bool debug=false)
        {
            Type loaderType = this.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var validFields  = loaderType.GetFields(flags).Where(x => x.GetCustomAttribute<MetaAttribute>() != null);
            var validProps   = loaderType.GetProperties(flags).Where(x => x.GetCustomAttribute<MetaAttribute>() != null);

            foreach(FieldInfo field in validFields)
            {
                if(debug)
                    Debug.Log($"Instantiating Field <{field.Name}> with type <{field.FieldType}>");
                
                var instance = Activator.CreateInstance(field.FieldType) as IBaseMetaFile;
                instance.Load();
                field.SetValue(this, instance);
            }

            foreach(PropertyInfo prop in validProps)
            {
                if(debug)
                    Debug.Log($"Instantiating Property <{prop.Name}> with type <{prop.PropertyType}>");
                
                var instance = Activator.CreateInstance(prop.PropertyType) as IBaseMetaFile;
                instance.Load();
                prop.SetValue(this, instance);
            }
        }
        #endregion
    }
}