using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Behaviour Tree Blackboard.
    /// The blackboard is a class that contains a set of shared data between
    /// all of it's children
    /// </summary>
    public class Blackboard
    {
        #region Fields
        private Dictionary<string, object> _properties;
        #endregion


        #region Constructors
        public Blackboard() => _properties = new Dictionary<string, object>();
        #endregion


        #region Methods
        /// <summary>
        /// Retrieves a property from the blackboard.
        /// Throws KeyNotFoundException if property doesnt exist.
        /// Throws InvalidCastException if property exists, but invalid type was used 
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <typeparam name="T">The property type</typeparam>
        /// <returns>The property with the name. Throws an exception if the property doesn't exist</returns>
        public T GetProperty<T>(string propertyName) => (T)_properties[propertyName];

        /// <summary>
        /// Checks if a given vlaue is present for a certain key
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if there's a value for the property. False otherwise</returns>
        public bool HasProperty(string propertyName) => _properties.ContainsKey(propertyName);

        /// <summary>
        /// Adds/Sets a property with a given name and value
        /// </summary>
        /// <param name="value">The property value</param>
        /// <param name="name">The property name</param>
        /// <typeparam name="T">The property type</typeparam>
        public void SetProperty<T>(T value, string name) {
            if(!_properties.ContainsKey(name))
                _properties.Add(name, value);
            else
                _properties[name] = value;
        } 
        #endregion
    }
}