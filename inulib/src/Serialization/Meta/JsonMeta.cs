using System;
using Newtonsoft.Json;

namespace INUlib.Serialization.Meta
{
    public abstract class JsonMeta<T> : IMetaFile<T> where T : class
    {
        #region Properties
        public T Data { get; protected set; }
        public abstract string FilePath { get; }
        #endregion Properties
        
        
        #region Methods
        protected abstract string ReadResourceFile(string filePath);

        public virtual bool Load()
        {
            var fileText = ReadResourceFile(FilePath);
            if (String.IsNullOrEmpty(fileText)) return false;
            
            Data = JsonConvert.DeserializeObject<T>(fileText);
            if (Data == null) return false;

            return true;
        }

        public virtual bool Load(JsonSerializerSettings serializeSettings)
        {
            var fileText = ReadResourceFile(FilePath);
            if (String.IsNullOrEmpty(fileText)) return false;
            
            Data = JsonConvert.DeserializeObject<T>(fileText, serializeSettings);
            if (Data == null) return false;

            return true;
        }
        #endregion Methods
    }
}