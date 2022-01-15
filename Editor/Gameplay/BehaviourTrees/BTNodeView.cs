using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using INUlib.Gameplay.AI.BehaviourTrees;

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
        #endregion

        #region Constructors
        public BTNodeView(SerializedNode node)
        {
            _node = node;
            title = node.name;

            style.left = node.pos.x;
            style.top = node.pos.y;
        }
        #endregion


        #region Methods
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            _node.pos.x = newPos.xMin;
            _node.pos.y = newPos.yMin;
        }
        #endregion
    }

}