using UnityEngine;
using Newtonsoft.Json.Linq;

namespace INUlib.BackendToolkit.Persistence.GameObjects.Surrogates
{
    public class TransformSurrogate : GameObjectSurrogate
    {
        #region Surrogate Methods
        public override JObject Save()
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            Vector3 scale = transform.localScale;
            
            JObject jObject = JObject.FromObject(new
            {
                position = new
                {
                    position.x,
                    position.y,
                    position.z,
                },
                rotation = new
                {
                    rotation.x,
                    rotation.y,
                    rotation.z,
                    rotation.w
                },
                scale = new
                {
                    scale.x,
                    scale.y,
                    scale.z
                }
            });
            
            return jObject;
        }
        
        public override bool Load(JObject saveable)
        {
            // JObject saveable = JObject.Parse(componentJsonString);
            float x = (float)saveable["position"]["x"];
            float y = (float)saveable["position"]["y"];
            float z = (float)saveable["position"]["z"];
            transform.position = new Vector3(x, y, z);
            
            x = (float)saveable["rotation"]["x"];
            y = (float)saveable["rotation"]["y"];
            z = (float)saveable["rotation"]["z"];
            float w = (float)saveable["rotation"]["w"];
            transform.rotation = new Quaternion(x, y, z, w);
            
            x = (float)saveable["scale"]["x"];
            y = (float)saveable["scale"]["y"];
            z = (float)saveable["scale"]["z"];
            transform.localScale = new Vector3(x, y, z);
            
            return true;
        }
        #endregion Surrogate Methods
    }
}