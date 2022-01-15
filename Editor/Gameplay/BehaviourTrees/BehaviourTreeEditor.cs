using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace INUlib.UEditor.Gameplay.BehaviourTrees
{
    public class BehaviourTreeEditor : EditorWindow
    {
        [MenuItem("INU lib/AI/Behaviour Tree")]
        public static void OpenWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            string visualTreePath = "Packages/com.ivanneves.inulib/Editor/Gameplay/BehaviourTrees/BehaviourTreeEditor.uxml";
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreePath);
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            string styleSheetPath = "Packages/com.ivanneves.inulib/Editor/Gameplay/BehaviourTrees/BehaviourTreeEditor.uss";
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
            
            root.styleSheets.Add(styleSheet);
        }
    }
}