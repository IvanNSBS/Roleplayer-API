using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public abstract class SerializedAction : SerializedBTNode 
    {
        /// <summary>
        /// Tries to add a child to the SerializedAction node.
        /// However, Actions can't have childs so it will always return false
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public override bool AddChild(SerializedBTNode child) => false;
    }
    public abstract class SerializedComposite : SerializedBTNode { }

    public abstract class SerializedDecorator : SerializedBTNode 
    {
        /// <summary>
        /// Sets the decorator child
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
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