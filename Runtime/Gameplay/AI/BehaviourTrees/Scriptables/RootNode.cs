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

            if(childs.Count == 0) 
                childs.Add(child);
            else
                childs[0] = child;

            return true;
        }

        public override bool RemoveChild(SerializedBTNode child)
        {
            if(_child != child)
                return false;
            
            _child = null;
            return base.RemoveChild(child);
        }

        public override BTNode CreateNode()
        {
            if(!_child)
                return null;
            
            return _child.CreateNode();
        }
    }
}