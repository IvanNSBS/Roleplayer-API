// using UnityEditor;
// using UnityEngine;
// using INUlib.BackendToolkit.Audio;
// using System.Reflection;
// using System.Collections.Generic;
// using System;

// namespace INUlib.UEditor.Core
// {
//     [CustomPropertyDrawer(typeof(AudioData))]
//     public class AudioDataDrawer : PropertyDrawer
//     {
//         #region Fields
//         private float _btnHeight = 40;
//         private float _padding = 2;
//         #endregion


//         #region Methods
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             EditorGUI.PropertyField(position, property, label, true);
//             if(property.isExpanded)
//             {
//                 float btnWidth = Screen.width*0.3f;
//                 float y = position.y + position.height - _btnHeight + _padding*0.5f;
//                 float x = Screen.width/2 - btnWidth/2;
//                 Rect btnRect = new Rect(x, y, btnWidth, _btnHeight*0.75f);

//                 if(GUI.Button(btnRect, "Play"))
//                 {
//                     AudioData data = GetTargetObjectOfProperty(property) as AudioData;
//                     Debug.Log("Id: " + data.Id);
//                     Debug.Log("Volume: " + data.Volume);
//                     Debug.Log("Clip: " + data.Clips);
//                     StopAllClips();
//                     PlayClip(data.Clips);
//                     // AudioData data = (GetValue(property) as List<AudioData>)[0];
//                     // Debug.Log("Audio Id: " + data.Id);
//                 }
//             }
//         }

//         public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
//         {
//             System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
//             System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
//             System.Reflection.MethodInfo method = audioUtilClass.GetMethod(
//                 "PlayPreviewClip",
//                 System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
//                 null,
//                 new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
//                 null
//             );
//             method.Invoke(
//                 null,
//                 new object[] { clip, startSample, loop }
//             );
//         }

//         public static void StopAllClips() {
//             Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
//             Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
//             MethodInfo method = audioUtilClass.GetMethod(
//                 "StopAllPreviewClips",
//                 BindingFlags.Static | BindingFlags.Public,
//                 null,
//                 new System.Type[]{},
//                 null
//             );
//             method.Invoke(
//                 null,
//                 new object[] {}
//             );
//         }
//         /// <summary>
//         /// Gets the object the property represents.
//         /// </summary>
//         /// <param name="prop"></param>
//         /// <returns></returns>
//         public object GetTargetObjectOfProperty(SerializedProperty prop)
//         {
//             if (prop == null) return null;

//             var path = prop.propertyPath.Replace(".Array.data[", "[");
//             object obj = prop.serializedObject.targetObject;
//             var elements = path.Split('.');
//             foreach (var element in elements)
//             {
//                 if (element.Contains("["))
//                 {
//                     var elementName = element.Substring(0, element.IndexOf("["));
//                     var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
//                     obj = GetValue_Imp(obj, elementName, index);
//                 }
//                 else
//                 {
//                     obj = GetValue_Imp(obj, element);
//                 }
//             }
//             return obj;
//         }

//         private object GetValue_Imp(object source, string name, int index)
//         {
//             var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
//             if (enumerable == null) return null;
//             var enm = enumerable.GetEnumerator();
//             //while (index-- >= 0)
//             //    enm.MoveNext();
//             //return enm.Current;

//             for (int i = 0; i <= index; i++)
//             {
//                 if (!enm.MoveNext()) return null;
//             }
//             return enm.Current;
//         }

//         private object GetValue_Imp(object source, string name)
//         {
//             if (source == null)
//                 return null;
//             var type = source.GetType();

//             while (type != null)
//             {
//                 var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
//                 if (f != null)
//                     return f.GetValue(source);

//                 var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
//                 if (p != null)
//                     return p.GetValue(source, null);

//                 type = type.BaseType;
//             }
//             return null;
//         }


//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             float height = EditorGUI.GetPropertyHeight(property, true); 
//             if(property.isExpanded)
//                 height += _btnHeight + _padding;

//             return height;
//         }
//         #endregion
//     }
// }
