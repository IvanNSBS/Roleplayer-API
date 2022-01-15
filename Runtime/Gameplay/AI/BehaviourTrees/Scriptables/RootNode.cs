using UnityEngine;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class RootNode : SerializedBTNode
    {
        [Header("Root Node")]
        [SerializeField] private SerializedBTNode _child;

        public override bool AddChild(SerializedBTNode child)
        {
            _child = child;
            return true;
        }

        public override BTNode CreateNode()
        {
            if(!_child)
                return null;
            
            return _child.CreateNode();
        }
    }
}