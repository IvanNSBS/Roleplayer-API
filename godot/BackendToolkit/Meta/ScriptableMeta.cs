using Godot;

namespace INUlib.BackendToolkit.Meta
{
    public abstract class ScriptableMeta<T> : IMetaFile<T> where T : Resource
    {
        #region Properties
        public T Data { get; protected set; }
        public abstract string FilePath { get; }
        #endregion Properties
        
        
        #region Methods
        public virtual bool Load()
        {
            Data = ResourceLoader.Load<T>(FilePath);  
            return Data != null;
        }
        #endregion Methods
    }
}