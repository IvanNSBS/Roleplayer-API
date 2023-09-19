using UnityEngine;

namespace INUlib.BackendToolkit.Persistence.GameObjects
{
    [DisallowMultipleComponent]
    public class PersistentSceneGameObject : PersistentGameObject
    {
        private void Start()
        {
            LevelIndex = gameObject.scene.buildIndex;
            RegisterToDataStore();
        }
    }
}