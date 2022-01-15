using UnityEngine;
using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public abstract class SerializedBTNode : ScriptableObject
    {
        public SerializedBTNode parent;
        public List<SerializedBTNode> childs = new List<SerializedBTNode>();
        public Vector2 pos;
        public string guid;

        public abstract BTNode CreateNode();
        public virtual bool AddChild(SerializedBTNode child)
        {
            child.parent = this;
            childs.Add(child);

            return true;
        }

        public bool RemoveChild(SerializedBTNode child)
        {
            bool removed = childs.Remove(child);
            if(removed)
                child.parent = null;
                
            return removed;
        }
    }
}