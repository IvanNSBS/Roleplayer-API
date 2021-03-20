using UnityEngine;

namespace Essentials.Persistence.Data
{
    [CreateAssetMenu(fileName = "Prefab Element", menuName = "Persistence/Prefab Manager Element", order = 0)]
    public class PrefabManagerElement : ScriptableObject
    {
        #region Fields
        [SerializeField] private string m_id;
        [SerializeField] private GameObject m_gameObject;
        #endregion Fields

        #region Properties
        public GameObject GameObject => m_gameObject;
        public string Id => m_id;
        #endregion Properties
        
        
        #region Methods
        public void SetPrefab(string newId, GameObject gameObjectReference)
        {
            m_id = newId;
            m_gameObject = gameObjectReference;
        }

        public void UpdateId(string newId)
        {
            m_id = newId;
        }
        #endregion Methods
    }
}