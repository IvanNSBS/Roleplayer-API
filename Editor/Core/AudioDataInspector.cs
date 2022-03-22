using UnityEditor;
using UnityEngine;
using INUlib.BackendToolkit.Audio;
using UnityEngine.UIElements;

namespace INUlib.UEditor.Core
{
    [CustomPropertyDrawer(typeof(AudioData))]
    public class AudioDataDrawer : PropertyDrawer
    {
        #region Fields
        private float _btnHeight = 40;
        #endregion


        #region Methods
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
            if(property.isExpanded)
            {
                float height = EditorGUI.GetPropertyHeight(property, true);
                float width = Screen.width*0.3f;
                float center = Screen.width/2 - width/2;
                Rect btnRect = new Rect(center, height + _btnHeight*1.5f, width, _btnHeight*0.75f);
                if(GUI.Button(btnRect, "Play"))
                {
                    Debug.Log("udiosasaduhsai");
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property, true); 
            if(property.isExpanded)
                height += _btnHeight;

            return height;
        }
        #endregion
    }
}
