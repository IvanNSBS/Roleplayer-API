using UnityEngine;

namespace INUlib.Core.Meta
{
    public abstract class ScriptableMeta<T> : IMetaFile<T> where T : ScriptableObject
    {
        #region Properties
        public T Data { get; protected set; }
        public abstract string FilePath { get; }
        #endregion Properties
        
        
        #region Methods
        public virtual bool Load()
        {
            Data = Resources.Load<T>(FilePath);

            return Data != null;
        }
        #endregion Methods
    }
}