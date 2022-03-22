using UnityEditor;
using UnityEngine;
using INUlib.BackendToolkit.Audio;
using System.Reflection;
using System.Collections.Generic;

namespace INUlib.UEditor.Core
{
    [CustomPropertyDrawer(typeof(AudioData))]
    public class AudioDataDrawer : PropertyDrawer
    {
        #region Fields
        private float _btnHeight = 40;
        private float _padding = 2;
        #endregion


        #region Methods
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
            if(property.isExpanded)
            {
                float btnWidth = Screen.width*0.3f;
                float y = position.y + position.height - _btnHeight + _padding*0.5f;
                float x = Screen.width/2 - btnWidth/2;
                Rect btnRect = new Rect(x, y, btnWidth, _btnHeight*0.75f);

                if(GUI.Button(btnRect, "Play"))
                {
                    // AudioData data = (GetValue(property) as List<AudioData>)[0];
                    // Debug.Log("Audio Id: " + data.Id);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property, true); 
            if(property.isExpanded)
                height += _btnHeight + _padding;

            return height;
        }

        public object GetValue(SerializedProperty property)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = GetFieldViaPath(parentType, property.propertyPath);
            Debug.Log("Field Info: " + fi.FieldType); 
            
            return fi.GetValue(property.serializedObject.targetObject);
        }

        public System.Reflection.FieldInfo GetFieldViaPath(System.Type type, string path)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var parent = type;
            var fi = parent.GetField(path, flags);
            var paths = path.Split('.');
    
            for (int i = 0; i < paths.Length; i++)
            {
                fi = parent.GetField(paths[i], flags);
    
                // there are only two container field type that can be serialized:
                // Array and List<T>
                if (fi.FieldType.IsArray)
                {
                    parent = fi.FieldType.GetElementType();
                    i += 2;
                    continue;
                }
    
                if (fi.FieldType.IsGenericType)
                {
                    parent = fi.FieldType.GetGenericArguments()[0];
                    i += 2;
                    continue;
                }
    
                if (fi != null)
                {
                    parent = fi.FieldType;
                }
                else
                {
                    return null;
                }
            }
    
            return fi;
        }
        #endregion
    }
}
