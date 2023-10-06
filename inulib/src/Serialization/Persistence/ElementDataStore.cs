
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace INUlib.Serialization.Persistence
{
    /// <summary>
    /// Data Store that manages IDataStoreElement
    /// </summary>
    public abstract class ElementDataStore<T> : DataStore where T : class, IDataStoreElement
    {
        #region Fields
        protected Dictionary<string, T> subscribers;
        #endregion Fields
        
        #region Constructor
        public ElementDataStore(SaveManager saveManager) : base(saveManager)
        {
            subscribers = new Dictionary<string, T>();
        }
        #endregion Constructor
        
        #region DataStore Methods
        public override JObject SerializeStoredData()
        {
            JObject obj = new JObject();
            
            foreach (var pair in subscribers)
                obj.Add(pair.Key, pair.Value.ToJson());
            
            dataStoreCache = obj;
            return obj;
        }

        public override bool DeserializeStoredObjectsFromCache()
        {
            foreach (var pair in subscribers)
            {
                if(!dataStoreCache.ContainsKey(pair.Key))
                    continue;
                         
                var value = dataStoreCache[pair.Key] as JObject;
                pair.Value.FromJson(value);
            }
            
            return true;
        }

        public override void ClearSaveStore()
        {
            subscribers.Clear();
            dataStoreCache = new JObject();
        }

        public override void RemoveStoredObjects() => subscribers.Clear();
        #endregion DataStore Methods

        
        #region Methods
        /// <summary>
        /// Adds a subscriber. Subscribers will be stored on save cache with a load event
        /// </summary>
        /// <param name="objectId">The unique object id to be stored</param>
        /// <param name="obj">The object to be stored</param>
        /// <param name="overwriteIfPresent">Whether or not to overwrite the object if theres a subscriber with the id</param>
        /// <returns>True if added. False otherwise</returns>
        public virtual bool AddSubscriber(string objectId, T obj, bool overwriteIfPresent = false)
        {
            if (subscribers.ContainsKey(objectId))
            {
                if (overwriteIfPresent)
                {
                    subscribers[objectId] = obj;
                    return true;
                }

                return false;
            }

            subscribers.Add(objectId, obj);
            return true;
        }

        /// <summary>
        /// Removes a subscriber from the subscriber hash
        /// </summary>
        /// <param name="objectId">The id of the object to be removed</param>
        /// <returns>False if object wasn't present in hash. True otherwise</returns>
        public virtual bool RemoveSubscriber(string objectId)
        {
            if (!subscribers.ContainsKey(objectId))
                return false;

            subscribers.Remove(objectId);
            return true;
        }
        
        /// <summary>
        /// Gets an objet in the subscriber hash
        /// </summary>
        /// <param name="objetId">The id of the object to find</param>
        /// <returns>The object if it exists. Null otherwise</returns>
        public T GetSubscriber(string objetId)
        {
            if (!subscribers.ContainsKey(objetId))
                return null;
            
            return subscribers[objetId];
        }
        #endregion Methods
    }
}