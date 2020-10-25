using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPGCore.FileManagement.SavingFramework.Surrogates
{
    public class EnabledSurrogate : MonoBehaviour, ISaveableData
    {
        #region Surrogate Methods
        public JObject Save()
        {
            JObject jObject = JObject.FromObject(new
            {
                gameObject.activeSelf
            });
            
            return jObject;
        }

        public bool Load(JObject saveable)
        {
            bool active = (bool)saveable["activeSelf"];
            gameObject.SetActive(active);
            
            return true;
        }
        #endregion Surrogate Methods
    }
}