using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Interface for a Decorator Node on the
    /// Behaviour Tree
    /// </summary>
    public abstract class DecoratorNode : BTNode
    {
        #region Fields
        protected BTNode _child;
        #endregion

        #region Properties
        public BTNode Child => _child;
        #endregion


        #region Constructors
        public DecoratorNode() { }
        public DecoratorNode(BTNode child) => SetChild(child);
        #endregion


        #region Methods
        /// <summary>
        /// Sets the Decorator Child
        /// </summary>
        /// <param name="child">The new Decorator Child</param>
        public void SetChild(BTNode child) { 
            _child = child;
            _child.SetParent(this); 
        }
        
        public override IReadOnlyList<BTNode> GetChildren() => new List<BTNode>{ _child };
        #endregion
    }
}