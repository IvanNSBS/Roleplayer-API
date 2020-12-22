using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPGCore.Persistence.GameObjects.Surrogates
{
    [System.Serializable]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererSurrogate : GameObjectSurrogate
    {
        #region Fields
        private SpriteRenderer m_spriteRenderer;
        #endregion Fields

        #region Surrogate Methods
        public override JObject Save()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            Color color = m_spriteRenderer.color;
            
            JObject jObject = JObject.FromObject(new
            {
                flipX = m_spriteRenderer.flipX,
                flipY = m_spriteRenderer.flipY,
                color = new
                {
                    color.r,
                    color.g,
                    color.b,
                    color.a
                },
            });
            
            return jObject;
        }

        public override bool Load(JObject saveable)
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            
            bool flipX = (bool)saveable["flipX"];
            bool flipY = (bool)saveable["flipY"];
            Color color = new Color((float)saveable["color"]["r"], (float)saveable["color"]["g"], 
                (float)saveable["color"]["b"], (float)saveable["color"]["a"]);

            m_spriteRenderer.flipX = flipX;
            m_spriteRenderer.flipY = flipY;
            m_spriteRenderer.color = color;
            
            return true;
        }
        #endregion Surrogate Methods
    }
}