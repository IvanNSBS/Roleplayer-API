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
        #region Constants
        private const string UXML_PATH = "Packages/com.ivanneves.inulib/Editor/Gameplay/BehaviourTrees/BTNodeView.uxml";
        #endregion


        #region Fields
        private SerializedBTNode _node;
        private Port _input;
        private Port _output;
        public event Action<BTNodeView> onNodeSelected = delegate { };
        #endregion

        #region Properties
        public SerializedBTNode SerializedNode => _node;
        #endregion


        #region Constructors
        public BTNodeView(SerializedBTNode node) : base(UXML_PATH)
        {
            _node = node;
            title = node.name;

            viewDataKey = node.guid;
            style.left = node.pos.x;
            style.top = node.pos.y;

            CreateInputPorts();
            CreateOutputPorts();
            AddUssClasses();
        }
        #endregion


        #region Methods
        private void CreateInputPorts()
        {
            if(_node is RootNode)
                return;

            _input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));

            if(_input != null) 
            {
                _input.portName = "";
                _input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(_input);
            }
        }

        private void CreateOutputPorts()
        {
            if(_node is SerializedAction) 
                _output = null;
            else if (_node is SerializedComposite)
                _output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            else if (_node is SerializedDecorator)
                _output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            else if (_node is RootNode)
                _output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));

            if(_output != null)
            {
                _output.portName = "";
                _output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(_output);
            }
        }

        private void AddUssClasses()
        {
            if(_node is SerializedAction) 
                AddToClassList("action");
            else if (_node is SerializedComposite)
                AddToClassList("composite");
            else if (_node is SerializedDecorator)
                AddToClassList("decorator");
            else if (_node is RootNode)
                AddToClassList("root");

        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            _node.pos.x = newPos.xMin;
            _node.pos.y = newPos.yMin;

            _node.parent?.SortChildren();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            onNodeSelected?.Invoke(this);
        }

        public Edge ConnectTo(Port input) => _output.ConnectTo(input);
        public Port GetInput() => _input;
        #endregion
    }

}