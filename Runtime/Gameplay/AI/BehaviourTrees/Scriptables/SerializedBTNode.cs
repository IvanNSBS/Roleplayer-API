using UnityEngine;
using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public abstract class SerializedBTNode : ScriptableObject
    {
        #region Fields
        [HideInInspector] public SerializedBTNode parent;
        [HideInInspector] public Vector2 pos;
        [HideInInspector] public string guid;
        [HideInInspector] public List<SerializedBTNode> childs = new List<SerializedBTNode>();
        #endregion


        #region Methods
        /// <summary>
        /// Instantiates a BTNode from the NodeFactory function and then creates it's children
        /// </summary>
        /// <returns></returns>
        public BTNode CreateNode()
        {
            var node = NodeFactory();

            if(childs.Count > 0)
            {
                if(node is DecoratorNode)
                    (node as DecoratorNode).SetChild(childs[0].CreateNode());
                else if(node is CompositeNode)
                    foreach(var child in childs)
                        (node as CompositeNode).AddChild(child.CreateNode());
            }

            return node;
        }

        /// <summary>
        /// Creates a BT Node for the SerializedBTNode
        /// </summary>
        /// <returns></returns>
        protected abstract BTNode NodeFactory();

        #if UNITY_EDITOR
        /// <summary>
        /// Adds a SerializedBTNode as child of this node
        /// </summary>
        /// <param name="child">The child to be added</param>
        /// <returns>True if the child was added, false otherwise</returns>
        public virtual bool AddChild(SerializedBTNode child)
        {
            UnityEditor.Undo.RecordObject(this, "Behaviour Tree (Add Child)");
            child.parent = this;
            childs.Add(child);
            SortChildren();

            UnityEditor.EditorUtility.SetDirty(this);
            return true;
        }

        /// <summary>
        /// Removes a child from the SerializedBTNode child list
        /// </summary>
        /// <param name="child">The child to be removed</param>
        /// <returns>True if the child was added. False otherwise</returns>
        public virtual bool RemoveChild(SerializedBTNode child)
        {
            UnityEditor.Undo.RecordObject(this, "Behaviour Tree (Remove Child)");

            bool removed = childs.Remove(child);
            if(removed) {
                child.parent = null;
                SortChildren();

            }

            UnityEditor.EditorUtility.SetDirty(this);
            return removed;
        }
        #endif

        /// <summary>
        /// Sorts the node childrens based on their editor X position
        /// </summary>
        public void SortChildren() => childs.Sort((a, b) => a.pos.x.CompareTo(b.pos.x));
        #endregion
    }
}