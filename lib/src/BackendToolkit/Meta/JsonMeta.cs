using System;
using Newtonsoft.Json;

namespace INUlib.BackendToolkit.Meta
{
    public abstract class JsonMeta<T> : IMetaFile<T> where T : class
    {
        #region Properties
        public T Data { get; protected set; }
        public abstract string FilePath { get; }
        #endregion Properties
        
        
        #region Methods
        public virtual bool Load()
        {
            var fileText = Resources.Load<TextAsset>(FilePath).text;
            if (String.IsNullOrEmpty(fileText)) return false;
            
            Data = JsonConvert.DeserializeObject<T>(fileText);
            if (Data == null) return false;

            return true;
        }

        public virtual bool Load(JsonSerializerSettings serializeSettings)
        {
            var fileText = Resources.Load<TextAsset>(FilePath).text;
            if (String.IsNullOrEmpty(fileText)) return false;
            
            Data = JsonConvert.DeserializeObject<T>(fileText, serializeSettings);
            if (Data == null) return false;

            return true;
        }
        #endregion Methods
    }
}