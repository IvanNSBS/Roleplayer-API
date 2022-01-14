using System.Collections.Generic;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
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
        protected override NodeState OnUpdate()
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