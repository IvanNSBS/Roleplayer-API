using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

namespace INUlib.UEditor.Gameplay.BehaviourTrees
{
    // This class exists so we can have a Node Graph View in the UI Builder, since it currently is
    // experimental and won't show up by default
    public class BehaviourTreeGraphView : GraphView
    {
        #region Uxml Factory
        public new class UxmlFactory : UxmlFactory<BehaviourTreeGraphView, GraphView.UxmlTraits> { }
        #endregion


        #region Constructors
        public BehaviourTreeGraphView() 
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer()); // lets you zoom in/ou the graph view
            this.AddManipulator(new ContentDragger()); // lets you pan around the graph view
            this.AddManipulator(new SelectionDragger()); // lets you select a node in the editor
            this.AddManipulator(new RectangleSelector()); // lets rectangle-select a bunch of nodes in the editor

            string styleSheetPath = "Packages/com.ivanneves.inulib/Editor/Gameplay/BehaviourTrees/BehaviourTreeEditor.uss";
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
            
            styleSheets.Add(styleSheet);
        }
        #endregion
    }

}