using Newtonsoft.Json.Linq;

namespace Essentials.Persistence.Interfaces
{
    /// <summary>
    /// Interface for objects that can be saved
    /// by the Saving System
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Function to serialize an object to json
        /// </summary>
        /// <returns>The Json representation of said object</returns>
        JObject Serialize();
        
        /// <summary>
        /// Function to load an object given it's json representation
        /// </summary>
        /// <param name="objectJson">The object json representation</param>
        /// <returns>True if object was loaded, false otherwise</returns>
        bool Deserialize(JObject objectJson);
    }
}