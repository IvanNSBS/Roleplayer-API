using UnityEditor;
using UnityEngine.UIElements;

namespace INUlib.UEditor.Gameplay.BehaviourTrees
{
    // Two Pane Split View is not Experimental, but for some reason it won't show up
    // in the default things to add, so we need to do the same as we did with the BT Graph View
    public class BTInspector : VisualElement
    {
        #region Uxml Factory
        public new class UxmlFactory : UxmlFactory<BTInspector, VisualElement.UxmlTraits> { }
        #endregion

        #region Fields
        private Editor _editor;
        #endregion


        #region Methods
        public void Update(BTNodeView inspect)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(_editor);

            _editor = Editor.CreateEditor(inspect.SerializedNode);
            IMGUIContainer container = new IMGUIContainer(_editor.OnInspectorGUI);
            Add(container);
        }
        #endregion
    }

}