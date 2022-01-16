using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public abstract class SerializedBTNode : ScriptableObject
    {
        #region Fields
        public SerializedBTNode parent;
        public List<SerializedBTNode> childs = new List<SerializedBTNode>();
        public Vector2 pos;
        public string guid;
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
        /// Creates a BT Node
        /// </summary>
        /// <returns></returns>
        protected abstract BTNode NodeFactory();

        public virtual bool AddChild(SerializedBTNode child)
        {
            Undo.RecordObject(this, "Behaviour Tree (Add Child)");
            child.parent = this;
            childs.Add(child);
            SortChildren();

            EditorUtility.SetDirty(this);
            return true;
        }

        public virtual bool RemoveChild(SerializedBTNode child)
        {
            Undo.RecordObject(this, "Behaviour Tree (Remove Child)");

            bool removed = childs.Remove(child);
            if(removed) {
                child.parent = null;
                SortChildren();

            }

            EditorUtility.SetDirty(this);
            return removed;
        }

        public void SortChildren() => childs.Sort((a, b) => a.pos.x.CompareTo(b.pos.x));
        #endregion
    }
}