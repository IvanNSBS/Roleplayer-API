namespace INUlib.Gameplay.AI.BehaviourTrees
{
    public class RepeaterDecorator : DecoratorNode
    {
        #region Fields
        private int _repeatAmnt;
        private int _repeatCount;
        #endregion


        #region Constructors
        public RepeaterDecorator() => _repeatAmnt = 1;
        public RepeaterDecorator(int repeat) => _repeatAmnt = repeat;
        public RepeaterDecorator(int repeat, BTNode child) : base(child) => _repeatAmnt = repeat;
        #endregion


        #region Methods
        protected override NodeState Evaluate()
        {
            if(_child == null) {
                _repeatCount++;
                return HasFinishedRepeating() ? NodeState.Success : NodeState.Running; 
            }

            if(_child.Update() == NodeState.Running)
                return NodeState.Running;

            _repeatCount++;
            return HasFinishedRepeating() ? _child.State : NodeState.Running; 
        }

        protected override void OnStart() => _repeatCount = 0;
        protected override void OnFinish() => _repeatCount = 0;
        #endregion


        #region Helper Methods
        private bool HasFinishedRepeating() => _repeatCount == _repeatAmnt;
        #endregion 
    }
}