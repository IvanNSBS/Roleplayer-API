using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using INUlib.Gameplay.AI.BehaviourTrees;
using System;

namespace INUlib.UEditor.Gameplay.BehaviourTrees
{
    // This class exists so we can have a Node Graph View in the UI Builder, since it currently is
    // experimental and won't show up by default
    public class BehaviourTreeGraphView : GraphView
    {
        #region Uxml Factory
        public new class UxmlFactory : UxmlFactory<BehaviourTreeGraphView, GraphView.UxmlTraits> { }
        #endregion

        #region Fields
        private BehaviourTreeAsset _btAsset;
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


        #region Overrides
        /// <summary>
        /// Builds the context menu when you right click the Graph View
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach(var type in types)
            {
                evt.menu.AppendAction($"{type.BaseType.Name}/ {type.Name}", a => {
                    InstantiateBTNode(type);
                });
            }

            types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach(var type in types)
            {
                evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", a => {
                    InstantiateBTNode(type);
                });
            }

            types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach(var type in types)
            {
                evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", a => {
                    InstantiateBTNode(type);
                });
            }
        }
        #endregion


        #region Methods
        internal void SetupView(BehaviourTreeAsset tree)
        {
            _btAsset = tree;

            // Clear previous tree
            DeleteElements(graphElements);

            _btAsset.InspectorNodes.ForEach(InstantiateNodeView);
        }

        internal void InstantiateNodeView(SerializedNode nodeView)
        {
            BTNodeView node = new BTNodeView(nodeView);
            AddElement(node);
        }

        private void InstantiateBTNode(Type t)
        {
            SerializedNode node = _btAsset.CreateNode(t);
            InstantiateNodeView(node);
        }
        #endregion
    }

}