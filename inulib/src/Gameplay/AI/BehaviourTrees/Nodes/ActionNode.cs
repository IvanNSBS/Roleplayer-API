using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Interface for an Action Node
    /// An action node is a BTNode that has no children
    /// </summary>
    public abstract class ActionNode : BTNode
    {
        #region Methods
        public override IReadOnlyList<BTNode> GetChildren() => null;
        #endregion
    }
}