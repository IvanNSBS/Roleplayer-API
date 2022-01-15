using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public abstract class SerializedAction : SerializedBTNode 
    {
        public override bool AddChild(SerializedBTNode child) => false;
    }
    public abstract class SerializedComposite : SerializedBTNode { }

    public abstract class SerializedDecorator : SerializedBTNode 
    {
        public override bool AddChild(SerializedBTNode child)
        {
            if(childs.Count == 1)
            {
                var oldChild = childs[0];
                RemoveChild(oldChild);
            }

            child.parent = this;
            childs.Add(child);
                
            return true;
        }
    }
}