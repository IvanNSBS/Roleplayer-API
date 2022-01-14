using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Sequence Node executes all of it's children until one 
    /// of them fails, making the sequence fail.
    /// A Sequence will succeed when all of it's children succeeds.
    /// It behaves like an AND operator
    /// </summary>
    public class SequenceNode : CompositeNode
    {
        #region Fields
        private int _currentChildIdx;
        #endregion

        #region Constructor
        public SequenceNode() : base() { }
        public SequenceNode(List<BTNode> childs) : base (childs) { }
        #endregion


        #region Methods
        protected override NodeState Evaluate()
        {
            if(GetChildren().Count == 0)
                return NodeState.Success;
            
            switch(_childs[_currentChildIdx].Update())
            {
                case NodeState.Running:
                    return NodeState.Running;

                case NodeState.Success:
                    _currentChildIdx++;
                    break;

                case NodeState.Failure:
                    return NodeState.Failure;
            }

            return HasVisitedAllChildrens() ? NodeState.Success : NodeState.Running;
        }

        protected override void OnStart() => _currentChildIdx = 0;
        protected override void OnFinish() => _currentChildIdx = 0;
        #endregion


        #region Helper Methods
        private bool HasVisitedAllChildrens() => _currentChildIdx == GetChildren().Count;
        #endregion
    }
}