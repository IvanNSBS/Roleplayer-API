using Newtonsoft.Json.Linq;

namespace RPGCore.FileManagement.SavingFramework
{
    /// <summary>
    /// Interface that defines how a Surrogate can save
    /// and load a Unity Component
    /// </summary>
    public interface ISaveableData
    {
        /// <summary>
        /// Property containing the name of the surrogate. Will be used
        /// to find it'sel in the json representation(JObject Key)
        /// </summary>
        string SurrogateName { get; }
        
        /// <summary>
        /// Bluerprint for the save method.
        /// Must define how the surrogate will gather it's component data
        /// and save it to a json representation
        /// </summary>
        /// <returns>JObject that contains the json representation of the surrogate data</returns>
        JObject Save();
        
        /// <summary>
        /// Blueprint for a surrogate to be able to load a component
        /// based on their json representation defined by the Save Method
        /// </summary>
        /// <param name="saveable">JObject that contains the json representation
        /// of the surrogate data. Must be the same as the result of the Save Method
        /// </param>
        /// <returns>True if the component were successfully loaded. False otherwise</returns>
        bool Load(JObject saveable);
    }
}