using UnityEngine;

namespace Essentials.Persistence.GameObjects
{
    [DisallowMultipleComponent]
    public class PersistentSceneGameObject : PersistentGameObject
    {
        private void Start()
        {
            LevelIndex = gameObject.scene.buildIndex;
            RegisterToDataStore<GameObjectStore>();
        }
    }
}