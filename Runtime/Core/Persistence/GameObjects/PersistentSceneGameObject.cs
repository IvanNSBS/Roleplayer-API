using UnityEngine;

namespace INUlib.Core.Persistence.GameObjects
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