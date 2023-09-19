using UnityEngine;
using Newtonsoft.Json.Linq;

namespace INUlib.BackendToolkit.Persistence.GameObjects
{
    /// <summary>
    /// Interface that defines how a Surrogate can save
    /// and load a Unity Component
    /// </summary>
    public abstract class GameObjectSurrogate : MonoBehaviour
    {
        /// <summary>
        /// Object unique identifier.
        /// OBS: Must be serialized to be changed in the Custom Editor
        /// </summary>
        [SerializeField][HideInInspector] private string m_id;

        /// <summary>
        /// Getter for object Unique Identifier
        /// </summary>
        public string Id
        {
            get => m_id;
            set => m_id = value;
        }
        
        /// <summary>
        /// Bluerprint for the save method.
        /// Must define how the surrogate will gather it's component data
        /// and save it to a json representation
        /// </summary>
        /// <returns>JObject that contains the json representation of the surrogate data</returns>
        public abstract JObject Save();
        
        /// <summary>
        /// Blueprint for a surrogate to be able to load a component
        /// based on their json representation defined by the Save Method
        /// </summary>
        /// <param name="saveable">JObject that contains the json representation
        /// of the surrogate data. Must be the same as the result of the Save Method
        /// </param>
        /// <returns>True if the component were successfully loaded. False otherwise</returns>
        public abstract bool Load(JObject saveable);
    }
}