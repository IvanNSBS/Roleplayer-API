using Newtonsoft.Json.Linq;

namespace Essentials.Persistence.GameObjects.Surrogates
{
    [System.Serializable]
    public class EnabledSurrogate : GameObjectSurrogate
    {
        #region Surrogate Methods
        public override JObject Save()
        {
            JObject jObject = JObject.FromObject(new
            {
                gameObject.activeSelf
            });
            
            return jObject;
        }

        public override bool Load(JObject saveable)
        {
            bool active = (bool)saveable["activeSelf"];
            gameObject.SetActive(active);
            
            return true;
        }
        #endregion Surrogate Methods
    }
}