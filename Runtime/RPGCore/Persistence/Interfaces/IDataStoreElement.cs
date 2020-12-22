namespace RPGCore.Persistence.Interfaces
{
    /// <summary>
    /// Interface for objects that can be saved
    /// by the Saving System
    /// </summary>
    public interface IDataStoreElement<T> : ISerializableObject where T : DataStore
    {
        /// <summary>
        /// Reference to a Data Store
        /// </summary>
        T DataStore { get; }
        
        /// <summary>
        /// Function to register the DataStore Element to a
        /// Save Manager DataStore
        /// </summary>
        /// <typeparam name="u"></typeparam>
        void RegisterToDataStore<U>() where U : T;
        void RemoveFromDataStore();
    }
}