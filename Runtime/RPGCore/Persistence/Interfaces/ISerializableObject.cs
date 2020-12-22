using Newtonsoft.Json.Linq;

namespace RPGCore.Persistence.Interfaces
{
    /// <summary>
    /// Interface for objects that can be saved
    /// by the Saving System
    /// </summary>
    public interface ISerializableObject : ISerializable
    {
        /// <summary>
        /// Object Unique Identifier
        /// </summary>
        string ObjectId { get; }
    }
}