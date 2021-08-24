using Newtonsoft.Json.Linq;

namespace Essentials.Persistence.Interfaces
{
    /// <summary>
    /// Interface for objects that can be saved
    /// by the Saving System
    /// </summary>
    public interface IDataStoreElement
    {
        /// <summary>
        /// Object Unique Identifier
        /// </summary>
        string ObjectId { get; }

        /// <summary>
        /// Function to serialize an object to json
        /// </summary>
        /// <returns>The Json representation of said object</returns>
        JObject ToJson();
        
        /// <summary>
        /// Function to load an object given it's json representation
        /// </summary>
        /// <param name="objectJson">The object json representation</param>
        /// <returns>True if object was loaded, false otherwise</returns>
        void FromJson(JObject objectJson);
        
        /// <summary>
        /// Function to register the DataStore Element to a
        /// Save Manager DataStore
        /// </summary>
        void RegisterToDataStore();
        void RemoveFromDataStore();
    }
}