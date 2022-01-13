using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
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
        public void AddChild(BTNode child) => _childs.Add(child);
        public bool RemoveChild(BTNode child) => _childs.Remove(child);
        #endregion
    }
}