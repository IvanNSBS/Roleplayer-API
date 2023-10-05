using Newtonsoft.Json.Linq;

namespace INUlib.Serialization.Persistence
{
    public abstract class DataStore
    {
        #region Fields
        protected JObject dataStoreCache;
        protected SaveManager saveManager;
        #endregion Fields
        
        #region Constructor
        protected DataStore(SaveManager saveManager)
        {
            this.saveManager = saveManager;
        }
        #endregion Constructor
        
        #region Methods
        /// <summary>
        /// Clears the data store cache
        /// </summary>
        public void ClearCache() => dataStoreCache.RemoveAll();

        /// <summary>
        /// Sets the data store cache. Used by the save manager.
        /// </summary>
        /// <param name="objectJson">The new cache value</param>
        public virtual void SetCache(JObject objectJson) => dataStoreCache = objectJson;
        #endregion Methods

        #region Abstract Methods
        /// <summary>
        /// Serializes the stored data present in the store
        /// </summary>
        /// <returns>The resulting JObject from the data store serialization</returns>
        public abstract JObject SerializeStoredData();
        
        /// <summary>
        /// Deserialize the subscribers with the current data store cache data
        /// </summary>
        /// <returns>True if properly loaded. False otherwise</returns>
        public abstract bool DeserializeStoredObjectsFromCache();
        
        /// <summary>
        /// Clears the DataStore, removing all stored data and resetting the cache
        /// </summary>
        public abstract void ClearSaveStore();

        /// <summary>
        /// Function to clear the objects that'll be stored once the save event is called
        /// </summary>
        public abstract void RemoveStoredObjects();
        #endregion Abstract Methods
    }
}