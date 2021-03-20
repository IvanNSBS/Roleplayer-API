using Newtonsoft.Json.Linq;

namespace Essentials.Persistence.Interfaces
{
    public abstract class DataStore : ISerializable
    {
        #region Fields
        protected JObject dataStoreCache;
        protected SaveManager saveManager;
        #endregion Fields
        
        #region Constructor
        protected DataStore(SaveManager saveManager)
        {
            dataStoreCache = new JObject();
            this.saveManager = saveManager;
        }
        #endregion Constructor
        
        #region Methods
        public abstract JObject Serialize();
        public abstract bool Deserialize(JObject objectJson);
        #endregion Methods
    }
}