using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPGCore.FileManagement.SavingFramework.Surrogates
{
    public class EnabledSurrogate : MonoBehaviour, ISaveableData
    {
        #region Properties
        public string SurrogateName => "Enabled";
        #endregion Properties
        
        #region Surrogate Methods
        public string Save()
        {
            JObject jObject = JObject.FromObject(new
            {
                this.gameObject.activeSelf
            });
            
            string jsonString = jObject.ToString();
            json = jsonString;
            return jsonString;
        }

        private string json;
        
        public bool Load(JObject saveable)
        {
            // JObject saveable = JObject.Parse(componentJsonString);
            bool active = (bool)saveable["activeSelf"];
            gameObject.SetActive(active);
            
            return true;
        }
        #endregion Surrogate Methods
    }
}