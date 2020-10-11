using UnityEngine;
using Newtonsoft.Json.Linq;

namespace RPGCore.FileManagement.SavingFramework.PrefabManager
{
    /// <summary>
    /// PrefabManagers must provide a way to retrieve a GameObject from a list.
    /// Prefabs that must be instantiated from SaveManager must be retrieved from
    /// a PrefabManager Component
    /// </summary>
    public interface IPrefabManager
    {
        /// <summary>
        /// Get a prefab from it's unique ID
        /// </summary>
        /// <param name="jsonKey">saveFile Saveable unique ID</param>
        /// <param name="objectJson">save file Saveable json data</param>
        /// <returns>The gameObject that corresponds to this ID. False if the id does not exist</returns>
        GameObject InstantiatePrefab(string jsonKey, JObject objectJson);
    }
}