using System;

namespace INUlib.Gameplay.AI.BehaviourTrees
{
    /// <summary>
    /// Repeater is a decorator that repeats the execution of a child
    /// node N times.
    /// </summary>
    [Serializable]
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
        protected override NodeState Evaluate(float deltaTime)
        {
            if(_child == null) {
                _repeatCount++;
                return HasFinishedRepeating() ? NodeState.Success : NodeState.Running; 
            }

            if(_child.Update(deltaTime) == NodeState.Running)
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