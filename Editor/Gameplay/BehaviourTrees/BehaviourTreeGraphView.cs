using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using INUlib.Gameplay.AI.BehaviourTrees;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    InstantiateBTNode(type, evt.localMousePosition);
                });
            }

            types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach(var type in types)
            {
                evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", a => {
                    InstantiateBTNode(type, evt.localMousePosition);
                });
            }

            types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach(var type in types)
            {
                evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", a => {
                    InstantiateBTNode(type, evt.localMousePosition);
                });
            }
        }

        // This function needs to be overriden, otherwise nodes connections just won't show up
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
            .Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node)
            .ToList();
        } 

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if(graphViewChange.elementsToRemove != null)
            {
                foreach(GraphElement remove in graphViewChange.elementsToRemove)
                {
                    BTNodeView nodeView = remove as BTNodeView;
                    if(nodeView != null)
                        _btAsset.RemoveNode(nodeView.SerializedNode);

                    Edge edge = remove as Edge;
                    if(edge != null)
                    {
                        BTNodeView parent = edge.output.node as BTNodeView;
                        BTNodeView child = edge.input.node as BTNodeView;

                        if(parent.SerializedNode.node is CompositeNode)
                        {
                            var composite = parent.SerializedNode.node as CompositeNode;
                            composite.RemoveChild(child.SerializedNode.node);
                        }
                        else if(parent.SerializedNode.node is DecoratorNode)
                        {
                            var composite = parent.SerializedNode.node as DecoratorNode;
                            composite.SetChild(null);
                        }
                    }
                }
            }

            if(graphViewChange.edgesToCreate != null)
            {
                foreach(Edge edge in graphViewChange.edgesToCreate)
                {
                    BTNodeView parent = edge.output.node as BTNodeView;
                    if(parent.SerializedNode.node is ActionNode)
                        continue;

                    BTNodeView child = edge.input.node as BTNodeView;
                    
                    if(parent.SerializedNode.node is CompositeNode)
                    {
                        var composite = parent.SerializedNode.node as CompositeNode;
                        composite.AddChild(child.SerializedNode.node);
                    }
                    else if(parent.SerializedNode.node is DecoratorNode)
                    {
                        var composite = parent.SerializedNode.node as DecoratorNode;
                        composite.SetChild(child.SerializedNode.node);
                    }
                }
            }

            return graphViewChange;
        }
        #endregion


        #region Methods
        internal void SetupView(BehaviourTreeAsset tree)
        {
            _btAsset = tree;

            graphViewChanged -= OnGraphViewChanged;
            // Clear previous tree
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            // Create Nodes
            _btAsset.InspectorNodes.ForEach(InstantiateNodeView);

            //Create Edges
            _btAsset.InspectorNodes.ForEach(CreateEdges);
        }

        internal void InstantiateNodeView(SerializedNode nodeView)
        {
            BTNodeView node = new BTNodeView(nodeView);
            AddElement(node);
        }

        private void InstantiateBTNode(Type t, Vector2 pos)
        {
            SerializedNode node = _btAsset.CreateNode(t);
            node.pos = pos;
            InstantiateNodeView(node);
        }

        private void CreateEdges(SerializedNode node)
        {
            var childs = node.node.GetChildren();
            
            if(childs != null && childs.Count > 0)
            {
                BTNodeView parentView = GetNodeByGuid(node.guid) as BTNodeView;
                foreach(var child in childs)
                {
                    var childView = GetNodeByGuid(child.guid) as BTNodeView;

                    var edge = parentView.ConnectTo(childView.GetInput());
                    AddElement(edge);
                }
            }
        }
        #endregion
    }

}