using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using INUlib.Gameplay.AI.BehaviourTrees;
using UnityEditor.Callbacks;

namespace INUlib.UEditor.Gameplay.BehaviourTrees
{
    public class BehaviourTreeEditor : EditorWindow
    {
        #region Fields
        private BehaviourTreeGraphView _view;
        private BTInspector _inspector;
        #endregion


        #region Editor Methods
        [MenuItem("INU lib/AI/Behaviour Tree")]
        public static void OpenWindow() => OpenWindow(null);
                
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            BehaviourTreeAsset btAsset = EditorUtility.InstanceIDToObject(instanceId) as BehaviourTreeAsset;
            if (btAsset != null)
            {   
                OpenWindow(btAsset);
                return true;
            }

            return false;
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
            
            _view = root.Q<BehaviourTreeGraphView>();
            _inspector = root.Q<BTInspector>();
            _view.SetUpdateInspectorCallback(_inspector.Update);

            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            BehaviourTreeAsset btAsset = Selection.activeObject as BehaviourTreeAsset;
            
            if(btAsset)
                _view.SetupView(btAsset);
        }
        #endregion


        #region Helper Methods
        private static void OpenWindow(BehaviourTreeAsset btAsset = null)
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");

            if(btAsset)
                wnd._view.SetupView(btAsset);
        }
        #endregion
    }
}