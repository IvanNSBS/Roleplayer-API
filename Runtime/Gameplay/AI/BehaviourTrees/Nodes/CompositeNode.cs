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
        protected List<BTNode> _childs;
        #endregion


        #region Properties
        public IReadOnlyList<BTNode> Childs => _childs;
        #endregion


        #region Constructors
        public CompositeNode() => _childs = new List<BTNode>();
        public CompositeNode(List<BTNode> childs) => _childs = childs;
        
        /// <summary>
        /// Adds a child to the Composite Node childs
        /// </summary>
        /// <param name="child">The child to be added</param>
        public void AddChild(BTNode child) => _childs.Add(child);
        
        /// <summary>
        /// Removes a child from the Composite Node
        /// </summary>
        /// <param name="child">The child to be removed</param>
        /// <returns>True if the child was present and removed. False otherwise</returns>
        public bool RemoveChild(BTNode child) => _childs.Remove(child);
        #endregion
    }
}