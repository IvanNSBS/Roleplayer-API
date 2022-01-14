using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Selector Node executes all of it's children until one 
    /// of them succeeds, making the Selector succeed.
    /// A Selector will fail when all of it's children fail.
    /// It behaves like an OR operator
    /// </summary>
    public class SelectorNode : CompositeNode
    {
        #region Fields
        private int _currentChildIdx;
        #endregion

        #region Constructor
        public SelectorNode() : base() { }
        public SelectorNode(List<BTNode> childs) : base (childs) { }
        #endregion


        #region Methods
        protected override NodeState Evaluate()
        {
            if(Childs.Count == 0)
                return NodeState.Success;
            
            switch(_childs[_currentChildIdx].Update())
            {
                case NodeState.Running:
                    return NodeState.Running;

                case NodeState.Success:
                    return NodeState.Success;

                case NodeState.Failure:
                    _currentChildIdx++;
                    break;
            }

            return HasVisitedAllChildrens() ? NodeState.Failure : NodeState.Running;
        }

        protected override void OnStart() => _currentChildIdx = 0;
        protected override void OnFinish() => _currentChildIdx = 0;
        #endregion

        
        #region Helper Methods
        private bool HasVisitedAllChildrens() => _currentChildIdx == Childs.Count;
        #endregion
    }
}