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
    public class BTNodeView : Node
    {
        #region Uxml Factory
        // public new class UxmlFactory : UxmlFactory<BehaviourTreeGraphView, GraphView.UxmlTraits> { }
        #endregion


        #region Fields
        private SerializedNode _node;
        private Port _input;
        private Port _output;
        #endregion

        #region Properties
        public SerializedNode SerializedNode => _node;
        #endregion


        #region Constructors
        public BTNodeView(SerializedNode node)
        {
            _node = node;
            title = node.name;

            viewDataKey = node.guid;
            style.left = node.pos.x;
            style.top = node.pos.y;

            CreateInputPorts();
            CreateOutputPorts();
        }
        #endregion


        #region Methods
        private void CreateInputPorts()
        {
            _input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));

            if(_input != null) 
            {
                _input.portName = "";
                inputContainer.Add(_input);
            }
        }

        private void CreateOutputPorts()
        {
            if(_node.node is ActionNode) 
                _output = null;
            else if (_node.node is CompositeNode)
                _output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            else if (_node.node is DecoratorNode)
                _output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

            if(_output != null)
            {
                _output.portName = "";
                outputContainer.Add(_output);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            _node.pos.x = newPos.xMin;
            _node.pos.y = newPos.yMin;
        }

        public Edge ConnectTo(Port input) => _output.ConnectTo(input);
        public Port GetInput() => _input;
        #endregion
    }

}