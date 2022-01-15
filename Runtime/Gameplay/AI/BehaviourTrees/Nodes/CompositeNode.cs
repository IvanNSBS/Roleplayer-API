using UnityEngine;
using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Interface for a Composite Node on Behaviour Tree
    /// A composite Node has N childrens
    /// </summary>
    public abstract class CompositeNode : BTNode
    {
        #region Fields
        [SerializeReference] protected List<BTNode> _childs;
        #endregion


        #region Constructors
        public CompositeNode() { }
        public CompositeNode(List<BTNode> childs) => _childs = childs;
        
        /// <summary>
        /// Adds a child to the Composite Node childs
        /// </summary>
        /// <param name="child">The child to be added</param>
        public void AddChild(BTNode child) {
            if(_childs == null)
                _childs = new List<BTNode>();

            _childs.Add(child);
            child.SetParent(this);
        }
        
        /// <summary>
        /// Removes a child from the Composite Node
        /// </summary>
        /// <param name="child">The child to be removed</param>
        /// <returns>True if the child was present and removed. False otherwise</returns>
        public bool RemoveChild(BTNode child) { 
            bool removed = _childs.Remove(child);
            if(removed)
                child.SetParent(null);

            return removed;
        }
        
        public override IReadOnlyList<BTNode> GetChildren() => _childs;
        #endregion
    }
}